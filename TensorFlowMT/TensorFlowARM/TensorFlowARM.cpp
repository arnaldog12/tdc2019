#include "TensorFlowARM.h"
#include "tensorflow/core/public/session.h"
#include "tensorflow/core/protobuf/meta_graph.pb.h"
#include "opencv2/core/core.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "TensorFlowARM", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "TensorFlowARM", __VA_ARGS__))

using namespace tensorflow;

template <class T> Tensor mat2tensor(cv::Mat image, tensorflow::DataType type)
{
	T *imageData = (T *)image.data;
	TensorShape imageShape = TensorShape{ 1, image.rows, image.cols, image.channels() };
	Tensor imageTensor = Tensor(type, imageShape);
	std::copy_n((char *)imageData, imageShape.num_elements() * sizeof(T), const_cast<char *>(imageTensor.tensor_data().data()));
	return imageTensor;
}

static std::vector<cv::Mat> tensor2mat(Tensor tensor)
{
	TensorShape shape = tensor.shape();
	int nDims = shape.dims();

	int nImages = (int)shape.dim_size(0);
	int width = (int)(nDims > 2 ? shape.dim_size(2) : (nDims > 1 ? shape.dim_size(1) : shape.dim_size(0)));
	int height = nDims > 2 ? (int)shape.dim_size(1) : 1;
	int channels = (nDims == 4) ? (int)shape.dim_size(3) : 1;

	std::vector<cv::Mat> result;
	for (int i = 0; i < nImages; i++)
	{
		Tensor slice = tensor.Slice(i, i + 1);
		assert(slice.IsAligned() == true);

		float *outputData = slice.flat<float>().data();
		cv::Mat imgOut(cv::Size(width, height), CV_32FC(channels));
		std::copy_n((char*)outputData, slice.shape().num_elements() * sizeof(float), (char*)imgOut.data);
		result.push_back(imgOut);
	}

	return result;
}

extern "C" 
{
	int predictDigit(unsigned char* data, int height, int width)
	{
		cv::Mat inputImg(cv::Size(width, height), CV_8UC4, data);
		cv::cvtColor(inputImg, inputImg, cv::COLOR_RGBA2GRAY);
		cv::resize(inputImg, inputImg, cv::Size(28, 28), 0, 0, cv::INTER_AREA);
		inputImg.convertTo(inputImg, CV_32F);
		
		GraphDef graphDef;
		TF_CHECK_OK(ReadBinaryProto(Env::Default(), "/storage/emulated/0/Android/data/AndroidApp.AndroidApp/files/best_model_ever.pb", &graphDef));

		Session *session;
		TF_CHECK_OK(NewSession(SessionOptions(), &session));
		TF_CHECK_OK(session->Create(graphDef));

		Tensor imgTensor = mat2tensor<float>(inputImg / 255.0f, tensorflow::DT_FLOAT);

		std::vector<std::pair<std::string, Tensor>> feedDict;
		feedDict.push_back({ "input_input", imgTensor });

		std::vector<Tensor> outputsTensor;
		TF_CHECK_OK(session->Run(feedDict, { "outputs/Softmax" }, {}, &outputsTensor));
		cv::Mat output = tensor2mat(outputsTensor[0])[0];

		cv::Point maxIdx;
		cv::minMaxLoc(output, NULL, NULL, NULL, &maxIdx);
		int y_pred = maxIdx.x;

		return y_pred;
	}
}

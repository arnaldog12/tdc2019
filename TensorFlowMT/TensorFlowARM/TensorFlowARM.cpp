#include "TensorFlowARM.h"
#include "tensorflow/core/public/session.h"
#include "tensorflow/core/protobuf/meta_graph.pb.h"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "TensorFlowARM", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "TensorFlowARM", __VA_ARGS__))

extern "C" 
{
	int predictDigit(cv::Mat frame)
	{
		cv::Mat inputImg;

		cv::cvtColor(frame, inputImg, cv::COLOR_BGR2GRAY);
		cv::resize(inputImg, inputImg, cv::Size(28, 28), 0, 0, cv::INTER_AREA);
		inputImg.convertTo(inputImg, CV_32F);
		return 0;
	}
}

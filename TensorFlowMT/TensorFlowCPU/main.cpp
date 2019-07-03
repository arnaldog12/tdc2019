#include "opencv2/core/core.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"

int main()
{
	cv::VideoCapture cap(1);
	if (!cap.isOpened())
		if (!cap.open(0)) return 0;

	cv::Mat frame;
	while (true)
	{
		cap >> frame;

		cv::imshow("frame", frame);
		if (cv::waitKey(10) == 27) break;
	}

	return 0;
}
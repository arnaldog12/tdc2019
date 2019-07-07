#pragma once

#include "opencv2/core/core.hpp"

#ifdef __cplusplus
// only need to export C interface if // used by C++ source code
extern "C"
{
#endif

	int predictDigit(cv::Mat frame);

#ifdef __cplusplus
}
#endif
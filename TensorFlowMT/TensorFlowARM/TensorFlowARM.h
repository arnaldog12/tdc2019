#pragma once

#ifdef __cplusplus
// only need to export C interface if // used by C++ source code
extern "C"
{
#endif

	int predictDigit(unsigned char* data, int height, int width);

#ifdef __cplusplus
}
#endif
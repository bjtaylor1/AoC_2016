#ifndef MD5HELPER
#define MD5HELPER

#include <openssl/md5.h>

std::string getmd5hash(std::string input)
{
	const unsigned char* c = (const unsigned char*)input.c_str();
	unsigned char result[MD5_DIGEST_LENGTH];
	MD5(c, input.size(), result);
	std::stringstream s;
	for (int i = 0; i < MD5_DIGEST_LENGTH; i++)
	{
		char output[4];
		sprintf_s(output, "%02x", result[i]);
		s << output;
	}
	return s.str();
}


#endif
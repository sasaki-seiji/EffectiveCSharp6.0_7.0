#pragma once
#include <string>
#include <iostream>
#include "B.h"

class Derived : public B
{
	std::string msg;

public:
	Derived(std::string msg) : msg("初期化子で設定")
	{
		this->msg = msg;
	}

protected:
	void VFunc() override
	{
		std::cout << msg;
	}
};
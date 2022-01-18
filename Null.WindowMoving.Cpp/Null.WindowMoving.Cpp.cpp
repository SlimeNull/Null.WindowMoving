// Null.WindowMoving.Cpp.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <Windows.h>

bool hotkeyDown = false;

HWND currentWindow;
int windowStartPointX, windowStartPointY;
int mouseStartPointX, mouseStartPointY;

void testHostkey()
{
	bool valueBefore = hotkeyDown;
	hotkeyDown = (GetAsyncKeyState(VK_LWIN) & GetAsyncKeyState(VK_LSHIFT)) != 0;

	if (hotkeyDown && !valueBefore)
	{
		currentWindow = GetForegroundWindow();
		POINT mouseP;
		RECT rect;
		if (GetCursorPos(&mouseP) && GetWindowRect(currentWindow, &rect))
		{
			mouseStartPointX = mouseP.x;
			mouseStartPointY = mouseP.y;
			windowStartPointX = rect.left;
			windowStartPointY = rect.top;
		}
		else
		{
			hotkeyDown = false;
		}
	}
}

void processWindowMoving()
{
	if (hotkeyDown)
	{
		POINT currentMousePoint;
		GetCursorPos(&currentMousePoint);
		int offsetX = currentMousePoint.x - mouseStartPointX,
			offsetY = currentMousePoint.y - mouseStartPointY;
		if (GetForegroundWindow() == currentWindow)
		{
			POINT newWindowPoint = { windowStartPointX + offsetX, windowStartPointY + offsetY };
			SetWindowPos(currentWindow, 0,
				windowStartPointX + offsetX,
				windowStartPointY + offsetY,
				-1, -1,
				SWP_NOSIZE | SWP_NOOWNERZORDER | SWP_NOSENDCHANGING);
		}
	}
}

int main()
{
	while (true)
	{
		testHostkey();
		processWindowMoving();
		Sleep(1);
	}
}
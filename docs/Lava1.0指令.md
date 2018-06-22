文件头(16字节):
```
4c 41 56 12 00 00 00 00 00 00 00 00 00 00 00 00
```


开头:
```
3c word16   ; 定义栈帧初始地址，一般是$2000，如果有全局变量则地址会增加，如定义了一个long全局变量，则运行地址为$2004
3b word24   ; 跳到main函数头
```


函数头：
```
3e word16 word8   ; word16表示局部变量字节数(包括参数，每个参数占4字节) + 返回地址(3字节) + 原来的栈帧基址(2字节)
                  ; word8表示参数个数
... (函数指令)
01 00             ; 如果函数需要返回值，就会在末尾自动加上return 0
3f                ; 函数返回
```


栈帧结构：
```
3字节 返回地址
2字节 原栈帧基址
... 各参数（每个4字节）
... 各局部变量（占用每个变量的类型所需的空间）
```


数据栈的slot是4字节。  
printf、sprintf函数调用时最后会加上01 xx，表示参数总个数。如printf("hello, %d", 134)最后会加上01 02。  
要把栈帧放在ram里，因为程序可能使用指向局部变量的指针。  
真值是-1。  


指令：

| 指令 | 参数 | 描述 |
|:---:|:----|:------|
| 00 | - | NOP |
| 01 | word8  | 把uint8压栈 |
| 02 | word16 | 把int16压栈 |
| 03 | word32 | 把uint32压栈 |
| 04 | word16(addr)   | 把addr处的uint8压栈(可能是全局变量地址空间、字符串地址空间、_TEXT、_GRAPH、_GBUF) |
| 05 | word16(addr)   | 把addr处的int16压栈 |
| 06 | word16(addr)   | 把addr处的uint32压栈 |
| 07 | word16(addr)   | 弹栈offset,把offset+addr处的uint8压栈 |
| 08 | word16(addr)   | 弹栈offset,把offset+addr处的int16压栈 |
| 09 | word16(addr)   | 弹栈offset,把offset+addr处的uint32压栈 |
| 0a | word16(addr)   | 弹栈offset,把offset+addr &#124; 0x00010000压栈，表示uint8指针(用于全局数组元素赋值) |
| 0b | word16(addr)   | 弹栈offset,把offset+addr &#124; 0x00020000压栈，表示int16指针 |
| 0c | word16(addr)   | 弹栈offset,把offset+addr &#124; 0x00040000压栈，表示uint32指针 |
| 0d | zstr           | 把字符串(以\0结尾)保存到字符串缓冲区, 并把该字符串保存的地址addr压栈，表示字符串指针 |
| 0e | word16(offset) | 把栈帧offset处的uint8压栈，即获取局部变量的值。 |
| 0f | word16(offset) | 把栈帧offset处的int16压栈，即获取局部变量的值。 |
| 10 | word16(offset) | 把栈帧offset处的uint32压栈，即获取局部变量的值。 |
| 11 | word16(offset) | 弹栈offset1,把栈帧offset+offset1处的uint8压栈(用于获取局部数组的元素) |
| 12 | word16(offset) | 弹栈offset1,把栈帧offset+offset1处的int16压栈 |
| 13 | word16(offset) | 弹栈offset1,把栈帧offset+offset1处的uint32压栈 |
| 14 | word16(offset) | 弹栈offset1,把栈帧基址+offset+offset1 &#124; 0x00010000压栈，即指向局部变量的指针(用于局部数组的元素赋值) |
| 15 | word16(offset) | 弹栈offset1,把栈帧基址+offset+offset1 &#124; 0x00020000压栈 |
| 16 | word16(offset) | 弹栈offset1,把栈帧基址+offset+offset1 &#124; 0x00040000压栈 |
| 17 | word16(addr)   | 弹栈offset,把addr+offset压栈 |
| 18 | word16(offset) | 弹栈offset1,把栈帧基址+offset+offset1压栈 |
| 19 | word16(offset) | 把栈帧基址+offset压栈(用于引用局部数组的地址) |
| 1a | - | _TEXT压栈 |
| 1b | - | _GRAPH压栈 |
| 1c | - | 弹栈int32, 取负数，压栈 |
| 1d | - | ++value. 弹栈指针,判断指针higher 16 bits, 把指针指向的value加1,并压栈新的value |
| 1e | - | --value. 弹栈指针,判断指针higher 16 bits, 把指针指向的value减1,并压栈新的value |
| 1f | - | value++. 弹栈指针,判断指针higher 16 bits, 把指针指向的value加1,并压栈旧的value |
| 20 | - | value--. 弹栈指针,判断指针higher 16 bits, 把指针指向的value减1,并压栈旧的value |
| 21 | - | 弹栈两个int32，相加，压栈 |
| 22 | - | 弹栈b,弹栈a,压栈a-b |
| 23 | - | bitwise-and |
| 24 | - | bitwise-or |
| 25 | - | bitwise-negation |
| 26 | - | bitwise-xor |
| 27 | - | &&，若结果为真则压栈-1，否则压栈0 |
| 28 | - | &#124;&#124;, 若结果为真则压栈-1, 否则压栈0 |
| 29 | - | !, 若结果为真则压栈-1, 否则压栈0 |
| 2a | - | 相乘 |
| 2b | - | 弹栈b, 弹栈a, 压栈a/b（除零会直接退出程序） |
| 2c | - | 取余（除0退出） |
| 2d | - | 左移 |
| 2e | - | 无符号右移 |
| 2f | - | == |
| 30 | - | != |
| 31 | - | <= |
| 32 | - | >= |
| 33 | - | > |
| 34 | - | < |
| 35 | - | 指针赋值。弹栈value，弹栈指针，根据指针higher 16 bits，把value赋值给指针指向的值。最后value压栈 |
| 36 | - | 弹栈word16(addr),取地址addr处的uint8压栈 |
| 37 | - | 弹栈word16(addr), addr &#124; 0x00010000压栈（把地址变成uint8指针，用于指针赋值） |
| 38 | - | 弹栈 |
| 39 | word24(pc) | 若刚刚弹栈的值为0则跳转到lav文件pc处继续执行 |
| 3a | word24(pc) | 若刚刚弹栈的值不为0则跳转到lav文件pc处继续执行 |
| 3b | word24(pc) | 跳转到lav文件pc处继续执行 |
| 3c | word16(addr) | 指定栈帧起始地址 |
| 3d | word24(pc1) | 调用函数。当前pc(3字节)保存到栈帧末尾后面(即下一栈帧开头)，然后跳到pc1处执行 |
| 3e | word16(frame_size) word8(argc) | 函数入口。把当前栈帧基址保存到栈帧末尾(上一条指令保存的pc后面)，把栈帧基址加上调用者函数的栈帧大小获得新的栈帧基址（新的栈帧紧跟在原来的栈帧后面）,最后把函数参数（从数据栈中获取）按从左往右放入栈帧对应位置 |
| 3f | - | 函数返回. 恢复栈帧基址和pc |
| 40 | - | 用于lav文件尾，标识main函数结束 |
| 41 | word16(addr) word16(len) word8...(data) | 初始化全局数组, 把len字节的data复制到地址addr处 |
| 42 | - | _GBUF压栈 |
| 43 | word8(secret) | 设置字符串xor加密的密码，使用0d保存字符串时会先xor secret。（用于加密lav文件的字符串常量？） |
| 44 | - | #loadall |
| 45 | int16(rhs) | 弹栈lhs,lhs+rhs压栈 |
| 46 | int16(rhs) | 弹栈lhs,lhs-rhs压栈 |
| 47 | int16(rhs) | 弹栈lhs,lhs*rhs压栈 |
| 48 | int16(rhs) | 弹栈lhs,lhs/rhs压栈 |
| 49 | int16(rhs) | 弹栈lhs,lhs%rhs压栈 |
| 4a | int16(rhs) | 弹栈lhs,lhs<<rhs压栈 |
| 4b | int16(rhs) | 弹栈lhs,lhs>>rhs（无符号右移）压栈 |
| 4c | int16(rhs) | 弹栈lhs,lhs==rhs压栈 |
| 4d | int16(rhs) | 弹栈lhs,lhs!=rhs压栈 |
| 4e | int16(rhs) | 弹栈lhs,lhs>rhs压栈 |
| 4f | int16(rhs) | 弹栈lhs,lhs<rhs压栈 |
| 50 | int16(rhs) | 弹栈lhs,lhs>=rhs压栈 |
| 51 | int16(rhs) | 弹栈lhs,lhs<=rhs压栈 |

函数：
**注：以下参数类型是Lava的参数类型，即char是无符号8bit整数，int是16字节整数，long是32字节整数**  
**数据栈栈顶是最右边的参数**

| 操作码 | 函数 | 说明 |
|:--:|:--|:--|
| 80 | void putchar(char ch) |  |
| 81 | char getchar() | 暂停程序，等待按键 |
| 82 | void printf(int, ...) | 支持%d, %c, %s, %%转义 |
| 83 | void strcpy(int dst,int src) | 两个字符串空间不能重叠 |
| 84 | int strlen(int) |  |
| 85 | void SetScreen(char mode) | 清屏并设置字体大小,mode=0大字体, =1小字体 |
| 86 | void UpdateLCD(char mode) | mode从高到低的bit代表是否刷新屏幕从上往下的每一行，0代表刷新，1不刷新 |
| 87 | void Delay(int value) | 延迟ms毫秒 |
| 88 | void WriteBlock(int x,int y,int width,int height,int type,int data) | type的bit6为1时直接在屏幕上作图<br>bit5为1时图形左右反转（要求x和width为8的倍数）<br>bit3为1时图形反显<br>bit2-0: 1:copy 2:not 3:or 4:and 5:xor |
| 89 | void Refresh() | 把图形缓冲区的内容刷新到屏幕 |
| 8A | void TextOut(int x,int y,int string,int type) | type的bit7=1大字体，=0小字体<br>bit6为1时直接在屏幕上绘图<br>bit5为1时所画图形左右反转（要求图形宽度和x坐标都必须是8的整数倍）<br>bit3为1时图形反显<br>bit2-0: 1:copy 2:not 3:or 4:and 5:xor  |
| 8B | void Block(int x0,int y0,int x1,int y1,int type) | 在缓冲区绘制实心矩形。<br>type: 0清除 1正常 2反相 |
| 8C | void Rectangle(int x0,int y0,int x1,int y1,int type) | 在缓冲区绘制空心矩形。<br>type: 0清除 1正常 2反相 |
| 8D | void exit(int exitcode)  | 忽略退出码 |
| 8E | void ClearScreen() | 清除缓冲区 |
| 8F | long abs(long x)    |  |
| 90 | int rand() |  |
| 91 | void srand(long seed) |  |
| 92 | void Locate(int y, int x) | x,y从0开始 |
| 93 | char Inkey() | 检测是否有键按下，如果有则返回键值否则返回0。立即返回 |
| 94 | void Point(int x,int y,int type) | type的bit6=1时向缓冲区作图，否则在屏幕上作图<br>bit0-2: 0清除 1正常 2反相 |
| 95 | int GetPoint(int x,int y) | 如果点存在则返回非零值 |
| 96 | void Line(int x0,int y0,int x1,int y1,int type) | type的bit6=1缓冲区作图,否则屏幕作图<br>bit0-2: 0清除 1正常 2反相 |
| 97 | void Box(int x0,int y0,int x1,int y1,int fill,int type) | 在屏幕上画矩形<br>type: 0清除 1正常 2反相<br>fill=0空心 =1实心 |
| 98 | void Circle(int x,int y,int r,int fill,int type) | 在屏幕上画圆<br>type: 0清除 1正常 2反相 |
| 99 | void Ellipse(int x,int y,int rx,int ry,int fill,int type) |  |
| 9A | void Beep() |  |
| 9B | int isalnum(char ch) |  |
| 9C | isalpha |  |
| 9D | iscntrl | 0x0-0x1f or 0x7f |
| 9E | isdigit |  |
| 9F | isgraph | 0x21-0x7e |
| A0 | islower |  |
| A1 | isprint | 0x20-0x7e |
| A2 | ispunct | 既不是字母数字，也不是空格的可打印字符 |
| A3 | isspace | space, tab, vtab, ff, lf, nl |
| A4 | isupper |  |
| A5 | isxdigit |  |
| A6 | void strcat(int dest,int src) |  |
| A7 | int strchr(int str,char c) |  |
| A8 | int strcmp(int str1,int str2) |  |
| A9 | int strstr(int str,int substr) |  |
| AA | char tolower(char ch) |  |
| AB | char toupper(char ch) |  |
| AC | void memset(int buffer,char c,int count) |  |
| AD | void memcpy(int dest,int src,int count) |  |
| AE | char fopen(int filename,int mode) | 如果打开失败则返回0<br> |
| AF | void fclose(char fp) |  |
| B0 | int fread(int ptr,int size,int n,char fp) | 返回读取的字节数，如果文件结束或出错则返回0<br>size参数会忽略，实际只使用n，建议为1 |
| B1 | int fwrite(int ptr,int size,int n,char fp) | 返回写入的字节数<br>size参数会忽略，实际只使用n |
| B2 | long fseek(char fp,long offset,char base) | 返回定位后的文件指针<br>base=0 SEEK_SET; =1 SEEK_CUR; =2 SEEK_END |
| B3 | long ftell(char fp) |  |
| B4 | int feof(char fp) |  |
| B5 | void rewind(char fp) |  |
| B6 | int getc(char fp) | 若文件结束或出错则返回-1 |
| B7 | int putc(char ch,char fp) | 返回写入的字符，若出错则返回-1 |
| B8 | sprintf(int str, int format, ...) |  |
| B9 | int MakeDir(int path) | 成功返回非零值，否则返回0 |
| BA | int DeleteFile(int filename) | 成功返回非零值 |
| BB | char Getms() | 返回1/256秒数 |
| BC | int CheckKey(char key) | 检测按键是否按下，若按下则返回-1，否则返回0<br>若key>=128则检测所有按键,如果有键按下则返回键值 |
| BD | void memmove(int dest,int src,int count) |  |
| BE | long Crc16(int mem,int len) |  |
| BF | void Secret(int mem,int len,int string) | 用string进行简单的xor加密 |
| C0 | int ChDir(int path) | 成功返回非0 |
| C1 | int FileList(int filename) | 列出当前目录的文件供用户选择<br>用户选择的文件名放入filename<br>用户放弃选择则返回0 |
| C2 | void GetTime(struct Time *t) | 使用GetTime,SetTime,请在程序里加上如下结构定义：<br>struct TIME<br>{<br>int year;<br>char month;<br> char day;<br>char hour;<br>char minute;<br>char second;<br>char week;<br>}; |
| C3 | void SetTime(struct Time *t) |  |
| C4 | long GetWord(int mode) | 等待输入一个宽字符.和getchar()不同的是可以输入中文<br>mode:=0 默认英文模式 =1 默认数字模式 =2 默认汉字模式 =3 保持以前的默认输入状态 |
| C5 | void XDraw(int mode) | 缓冲区全屏特效<br>mode: =0 缓冲区左移1个像素 =1 右移1像素 =4 左右反转 =5 上下反转 |
| C6 | void ReleaseKey(char key) | 把指定的按键状态改为释放状态（即使该键正按下）<br>getchar和Inkey对于持续按下的键只产生一个键值，使用ReleaseKey可以产生连续按键<br>若key>=128则释放所有按键 |
| C7 | void GetBlock(int x,int y,int width,int height,int type,int data) | type:=0 从缓冲区复制图形 =0x40 从屏幕复制<br>x和width忽略低3位 |
| C8 | int Sin(int deg) | deg是角度，取0~32767之间, 返回-1024~1024 |
| C9 | Cos |  |
| CA | void FillArea(int x,int y,int type) | 填充闭合的凸区域<br>type: =0 缓冲区 =0x40 屏幕 |

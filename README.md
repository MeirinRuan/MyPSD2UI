# MyPSD2UI

## 简介
你是否也遇到过这样的问题：开发过程中需要和UIUE对接，花费大量时间在拼接摆放界面控件和解决美术对控件坐标的强迫症似的精确要求。为了更好的把时间用在写代码(~~摸鱼~~)上，这个项目应运而生(~~如果美术愿意自己拼UI的话那就不需要了~~)。

## 功能
1. 增加了对图层组(layergroup)的解析支持。可以根据图层组进行分类，获取对应图层组下的所有图层。
2. 可以根据图层组生成对应的.ini配置文件。目前支持以下几种类型：
	- Wnd
	- Text
	- Button
	- Combo
	- Progress
3. 支持手动选择需要生成的图层。(美术提供的PS文件中可能存在许多在最终呈现上不需要显示的图层)
4. 支持读取已有的.ini配置文件，和PS文件的图层进行对比，同步对齐图层坐标。(开发过程中不一定每次都是美术先出好图再开始拼界面写代码，很多时候是代码先行的，暂代的界面和最终美术的界面可能存在差异，而且美术还经常改图)
5. 根据PS文件生成简易的UI预览。
6. 根据.ini配置文件生成lua代码。

## 使用方法

### 1. 生成ui配置
#### 首先为PS文件中的图层或图层组进行标识，然后打开psd文件，接着输入界面id，最后生成ui配置。PS文件标识规则如下（图层名和标识用下划线隔开）：

	//生成的图层为Text类型
	图层1_Text

	//生成的图层为Button类型
	图层2_Button

	//标识为Bg的图层为背景，控件id会设置为1，放在最底层
	图层3_Bg

	//标识为Null的图层不会在配置文件中生成
	图层4_Null

	//标识为Combo的图层组支持在其图层组下的图层中用_H和_W来标识子图层为列或者行
	//以下图层规则会生成一个控件"图层5_Combo"，其中有2x2的子控件，子图层6和子图层7为列，子图层8和子图层9为行
	图层5_Combo
	  子图层6_H
	  子图层7_H
	  子图层8_W
	  子图层9_W

	//一般情况只有单张图片的图层可以标识为Pic，目前也会生成Text类型
	图层10_Pic
	
支持的类型参考[功能](#功能)，未标识的情况下默认生成Text类型的控件配置。

### 2. 控件同步
#### 首先为已有的ini配置文件添加Comment字段的配置，然后读取ini配置，点击控件同步可以同步全部配置或右键列表中的任意项同步单个配置。
- 控件同步会将PS文件中命名和.ini配置中一致的图层全部同步到.ini配置中，这个功能需要PS文件的图层名和.ini配置中的Comment字段具有相同的命名。
- 右键.ini配置的列表中的任意配置可以进行单个配置的同步，这个功能不需要命名一致，可以选择任意你想要同步的PS图层进行同步。


## 引用的项目
- [PsdPlugin](https://github.com/PsdPlugin/PsdPlugin)
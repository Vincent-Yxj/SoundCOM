# SoundCOM Project

![image](https://github.com/Vincent-Yxj/SoundCOM/blob/main/pic.png)

## 项目简介

SoundCOM项目是一个用于音频测量的应用程序。它通过串口与测量设备通信，实时获取音频数据，提供实时测量图表和数据记录功能。

## 项目结构

### 1. SoundCOM

主要应用程序，包含图形用户界面和串口通信逻辑。

#### 主要特性

- 实时音频测量
- 数据记录与图表展示
- 串口设备连接与切换
- 用户设置页面

#### 使用的技术

- WPF (Windows Presentation Foundation)
- C#编程语言

### 2. SoundCOM.ViewModels

包含项目中使用的ViewModels，用于处理用户界面逻辑和与后台服务的交互。

### 3. SoundCOM.Service

提供后台服务，包括串口通信、USB设备监听、数据记录、颜色配置等。

#### 主要类

- SerialPortService：负责串口通信的服务。
- UsbDeviceListener：监听USB设备的插拔事件。
- DataGridService：数据记录服务。
- ColorService：颜色配置服务。

### 4. SoundCOM.Views

包含应用程序的各个页面，包括主界面和设置窗口。

## 使用说明

1. 打开SoundCOM应用程序。
2. 在设置窗口中选择串口和其他参数。
3. 点击“保存”按钮以应用设置。
4. 在主界面中开始测量，并查看实时测量数据和图表。

## 运行环境

 Windows

## 依赖项

 .NET 7

## 开发者

 作者：[Vincent-Yxj]

## License
 MIT

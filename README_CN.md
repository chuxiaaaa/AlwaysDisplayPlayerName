# AlwaysDisplayPlayerName

[English](README.md) | [中文](README_CN.md)

一个用于PEAK游戏的BepInEx模组，可以始终显示其他玩家的名称和距离信息。

## 功能特性

- **始终显示玩家名称**: 无论是否看向其他玩家，都会显示其名称
- **距离显示**: 显示当前玩家与其他玩家之间的距离
- **可配置选项**: 
  - 启用/禁用模组
  - 设置可见角度
  - 盲人状态下的显示控制
  - 距离显示开关
- **性能优化**: 使用间隔更新机制，减少性能开销

## 安装说明

### 前置要求
- PEAK游戏
- BepInEx 5.4.2403或更高版本

### 安装步骤
1. 下载模组文件
2. 将`com.github.yueby.AlwaysDisplayPlayerName.dll`放入游戏的`BepInEx/plugins/`目录
3. 启动游戏，模组将自动加载

### 推荐安装方式
推荐使用Thunderstore模组管理器安装，或从[Thunderstore模组页面](https://thunderstore.io/c/peak/p/Yueby/AlwaysDisplayPlayerName/)手动下载。

## 配置选项

模组提供以下配置选项（可在游戏内或配置文件中调整）：

- **Enable**: 启用/禁用模组
- **VisibleAngle**: 设置可见角度（默认52度）
- **DisplayWhenBlind**: 盲人状态下是否显示名称
- **ShowDistance**: 是否显示距离信息

## 技术实现

- 使用Harmony库进行游戏代码补丁
- 基于Unity的UI系统实现距离显示
- 采用组件化设计，便于维护和扩展

## 贡献

欢迎提交Issue和Pull Request来改进这个模组！

## 相关链接

- [Thunderstore模组页面](https://thunderstore.io/c/peak/p/Yueby/AlwaysDisplayPlayerName/) - 下载、评分和社区讨论
- [PEAK模组开发指南](https://peakmodding.github.io/getting-started/overview/) - 学习如何为PEAK游戏创建模组

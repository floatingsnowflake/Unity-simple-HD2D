Aseprite Importer 通过读取 Aseprite 文件，可以自动将 Layer，Tag，Tileset 导入成 Sprite，Animation。

Aseprite Importer 目前有3个非常有效的功能。

导入Aseprite的文件时:
* 从 layer 中导入成 sprite
* 从 tag 中导入成 animation
* 从 1.3 版本的 tileset 中导入成 sprite

以上功能使用时，只需要在 Inspector 中进行配置，选择相应的参数即可完成。
而且去除了繁杂的 Texture/Sprite 的配置项，对于新手来说非常方便。

同时，也可以作为从美术到程序的很好的工作流程的基础工具。

Note: 这个插件直接读取Aseprite内容，无需配置Aseprite的路径

# 使用

1. 将 Aseprite 文件拖入 Unity 项目的 Assets 任意位置
2. 选中这个文件
3. 在 Inspector 中配置需要导入的参数
4. 配置参数（请看后面）
5. 点击 Apply

Import Flags 选择 LayerToSprite 时：

4.1. 在 Layers Importing 中添加子项
4.2. 在子项中选择对应的 layer

Import Flags 选择 TagToAnimation 时：

4.1. 在 Animations Importing 中添加子项
4.2. 在子项中选择对应的 tag
4.3. loop: 配置动画是否循环播放


Import Flags 选择 Tileset 时：

4.1 在 Tileset Importing 中添加子项
4.2. 在子项中选择对应的 tileset

# 联系方式

Email: zzmingo88@gmail.com
Twitter: @MingoZ88







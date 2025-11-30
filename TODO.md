# 需求

实现一个本地音乐的播放器。

## TODO

- 国际化
- 增加log输出
- 显示歌曲更多信息
- 列表平滑滚动
- 更多设置
- 更多工具
- 弹窗背景模糊
- 队列中已有歌曲但是路径不存在或者无法播放
- 歌词显示
- 音频显示动效

## 界面

- 播放界面 -> 播放队列
- 修改播放列表界面，扫描所有歌曲
- 设置界面
- 播放时长和次数统计界面
- 搜索界面
- 设置歌曲文件信息

## domain

```mermaid
classDiagram
class MusicFile {
	path
}

class MusicList {
	files
}

class AggregationList {
	lists
}

class DirectoryList {
	dir
}

class FilesList {
	files
}

class MusicQueue {
	files
}

class PlayerState {
	currMusicQueue
	currMusicFile
	config
}

class MusicPlayer {
	play()
	stop()
	pause()
}

class FilePath {
	path:	string
}

class Setting {
	setting
}
MusicList "1" --> "0..*" MusicFile : has
MusicPlayer "1" --> "1" MusicQueue : use
MusicPlayer "1" --> "1" PlayerState : has
MusicQueue  --> MusicList : create from
MusicQueue  --> FilePath : create from
AggregationList  --|> MusicList: extends
DirectoryList  --|> MusicList: extends
FilesList  --|> MusicList: extends
```

## Architecture

```mermaid
graph TD
	A[User]
    A --> B
    A --> C
    A --> D
    B --> B1
    C --> C1
    D --> D1
    B1 --> E
    B1 --> F
    C1 --> G
    D1 --> E
    D1 --> G
    E --> H

	
    subgraph view
    B(Page1) 
    C(Page2) 
    D(Page3)
    end
    
    subgraph view model
    B1(Page1 vm) 
    C1(Page2 vm) 
    D1(Page3 vm)
    end
    
    subgraph service
    E(Service1)
	F(Service2)
	G(Service3)  
    end
    
    subgraph base
    H(file system)
    end

```



## Usercase

```mermaid
graph LR

    subgraph System[Orchitic]
        UC1(歌曲播放操作)
        UC2(队列操作)
        UC3(列表操作)
        UC4(文件信息操作)
        UC5(设置操作)
        
        UC1 --> UC1a(播放和暂停)
        UC1 --> UC1b(调节音量)
        UC1 --> UC1c(调节循环播放模式)
        UC1 --> UC1d(调节播放时间)
        UC1 --> UC1e(调节下一首 上一首)
        
        UC2 --> UC2a(搜索队列中的歌曲)
        UC2 --> UC2b(选择队列歌曲进行播放)
        UC2 --> UC2c(给队列新增额外的歌曲，或者删除歌曲)
        UC2 --> UC2d(从列表加载歌曲或者从文件加载歌曲)
        
        UC3 --> UC3a(从文件新建文件列表)
        UC3 --> UC3b(给文件列表新添，删除文件)
        UC3 --> UC3c(从文件夹新建目录列表)
        UC3 --> UC3d(新建聚合列表)
        UC3 --> UC3e(给聚合列表添加列表和删除列表)
        
        UC3 --> UC3f(加载某个列表到队列)
        UC3 --> UC3g(搜索列表)        
        
        UC5 --> UC5a(设置主题)
        UC5 --> UC5b(显示版本信息以及依赖的库信息)
    end

    User --> UC1
    User --> UC2
    User --> UC3
    User --> UC4
    User --> UC5
```


SET NAMES utf8mb4;
SET TIME_ZONE='+08:00';

CREATE TABLE IF NOT EXISTS `Discussions_Feedback`
(
	`FeedbackId`  bigint unsigned  NOT NULL COMMENT '主键，反馈编号',
	`SiteId`      int unsigned     NOT NULL COMMENT '站点编号',
	`Kind`        tinyint unsigned NOT NULL COMMENT '反馈种类',
	`Subject`     varchar(50)      NOT NULL COMMENT '反馈标题' COLLATE 'utf8mb4_0900_ai_ci',
	`Content`     varchar(500)     NOT NULL COMMENT '反馈内容' COLLATE 'utf8mb4_0900_ai_ci',
	`ContentType` varchar(100)     NULL     COMMENT '内容类型' COLLATE 'ascii_general_ci',
	`ContactName` varchar(50)      NOT NULL COMMENT '联系人名称' COLLATE 'utf8mb4_0900_ai_ci',
	`ContactText` varchar(50)      NOT NULL COMMENT '联系人方式' COLLATE 'utf8mb4_0900_ai_ci',
	`CreatedTime` datetime         NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`FeedbackId`)
) ENGINE=InnoDB COMMENT='反馈表';

CREATE TABLE IF NOT EXISTS `Discussions_Message`
(
	`MessageId`   bigint unsigned NOT NULL COMMENT '主键，消息编号',
	`SiteId`      int unsigned    NOT NULL COMMENT '站点编号',
	`Subject`     varchar(50)     NOT NULL COMMENT '消息标题' COLLATE 'utf8mb4_0900_ai_ci',
	`Content`     varchar(500)    NOT NULL COMMENT '消息内容' COLLATE 'utf8mb4_0900_ai_ci',
	`ContentType` varchar(100)    NULL     COMMENT '内容类型' COLLATE 'ascii_general_ci',
	`MessageType` varchar(50)     NULL     COMMENT '消息类型' COLLATE 'ascii_general_ci',
	`Referer`     varchar(50)     NULL     COMMENT '消息来源' COLLATE 'ascii_general_ci',
	`Tags`        varchar(100)    NULL     COMMENT '标签集' COLLATE 'utf8mb4_0900_ai_ci',
	`CreatorId`   int unsigned    NOT NULL COMMENT '创建人编号',
	`CreatedTime` datetime NOT    NULL     COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`MessageId`)
) ENGINE=InnoDB COMMENT='消息表';

CREATE TABLE IF NOT EXISTS `Discussions_Folder`
(
	`FolderId`     int unsigned     NOT NULL COMMENT '主键，目录编号',
	`SiteId`       int unsigned     NOT NULL COMMENT '站点编号',
	`Name`         varchar(50)      NOT NULL COMMENT '目录名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Acronym`      varchar(50)      NULL     COMMENT '名称缩写' COLLATE 'utf8mb4_0900_ai_ci',
	`Icon`         varchar(50)      NULL     COMMENT '显示图标' COLLATE 'ascii_general_ci',
	`Shareability` tinyint unsigned NOT NULL COMMENT '可共享性',
	`CreatorId`    int unsigned     NOT NULL COMMENT '创建人编号',
	`CreatedTime`  datetime         NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	`Description`  varchar(500)     NULL     COMMENT '描述文本' COLLATE 'utf8mb4_0900_ai_ci',
	PRIMARY KEY (`FolderId`)
) ENGINE=InnoDB COMMENT='目录表';

CREATE TABLE IF NOT EXISTS `Discussions_FolderUser`
(
	`FolderId`   int unsigned     NOT NULL COMMENT '主键，目录编号',
	`UserId`     int unsigned     NOT NULL COMMENT '主键，用户编号',
	`Permission` tinyint unsigned NOT NULL COMMENT '权限定义',
	`Expiration` datetime         NULL     COMMENT '过期时间',
	PRIMARY KEY (`FolderId`, `UserId`),
	CONSTRAINT `FK_FolderUser.FolderId` FOREIGN KEY (`FolderId`) REFERENCES `Discussions_Folder` (`FolderId`) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='目录用户表';

CREATE TABLE IF NOT EXISTS `Discussions_File`
(
	`FileId`      bigint unsigned NOT NULL COMMENT '主键，文件编号',
	`SiteId`      int unsigned    NOT NULL COMMENT '所属站点编号' DEFAULT 0,
	`FolderId`    int unsigned    NOT NULL COMMENT '所属目录编号' DEFAULT 0,
	`Name`        varchar(50)     NOT NULL COMMENT '文件名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Acronym`     varchar(50)     NULL     COMMENT '名称缩写' COLLATE 'utf8mb4_0900_ai_ci',
	`Path`        varchar(200)    NOT NULL COMMENT '文件路径' COLLATE 'utf8mb4_0900_ai_ci',
	`Type`        varchar(100)    NULL     COMMENT '文件类型' COLLATE 'ascii_general_ci',
	`Size`        int unsigned    NOT NULL COMMENT '文件大小',
	`Tags`        varchar(100)    NULL     COMMENT '标签集' COLLATE 'utf8mb4_0900_ai_ci',
	`CreatorId`   int unsigned    NOT NULL COMMENT '创建人编号',
	`CreatedTime` datetime        NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	`Description` varchar(500)    NULL     COMMENT '描述文本' COLLATE 'utf8mb4_0900_ai_ci',
	PRIMARY KEY (`FileId`)
) ENGINE=InnoDB COMMENT='文件（附件）表';

CREATE TABLE IF NOT EXISTS `Discussions_Site`
(
	`SiteId`      int unsigned NOT NULL COMMENT '主键，站点编号',
	`SiteNo`      varchar(50)  NOT NULL COMMENT '站点代号' COLLATE 'ascii_general_ci',
	`Name`        varchar(50)  NOT NULL COMMENT '站点名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Host`        varchar(50)  NOT NULL COMMENT '站点域名' COLLATE 'ascii_general_ci',
	`Icon`        varchar(100) NULL     COMMENT '显示图标' COLLATE 'ascii_general_ci',
	`Domain`      varchar(50)  NOT NULL COMMENT '所属领域' COLLATE 'ascii_general_ci',
	`Description` varchar(500) NULL     COMMENT '描述文本' COLLATE 'utf8mb4_0900_ai_ci',
	PRIMARY KEY (`SiteId`),
	UNIQUE INDEX `UX_SiteNo` (`SiteNo`),
	INDEX `IX_Host` (`Host`),
	INDEX `IX_Domain` (`Domain`)
) ENGINE=InnoDB COMMENT='站点表';

CREATE TABLE IF NOT EXISTS `Discussions_SiteUser`
(
	`SiteId` int unsigned NOT NULL COMMENT '主键，站点编号',
	`UserId` int unsigned NOT NULL COMMENT '主键，用户编号',
	PRIMARY KEY (`SiteId`, `UserId`),
	CONSTRAINT `FK_SiteUser.SiteId` FOREIGN KEY (`SiteId`) REFERENCES `Discussions_Site` (`SiteId`) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='站点用户表';

CREATE TABLE IF NOT EXISTS `Discussions_ForumGroup`
(
	`SiteId`      int unsigned      NOT NULL COMMENT '主键，站点编号',
	`GroupId`     smallint unsigned NOT NULL COMMENT '主键，分组编号',
	`Name`        varchar(50)       NOT NULL COMMENT '论坛组名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Icon`        varchar(100)      NULL     COMMENT '显示图标' COLLATE 'ascii_general_ci',
	`Ordinal`     smallint          NOT NULL COMMENT '排列顺序',
	`Description` varchar(500)      NULL     COMMENT '描述文本' COLLATE 'utf8mb4_0900_ai_ci',
	PRIMARY KEY (`SiteId`, `GroupId`)
) ENGINE=InnoDB COMMENT='论坛分组表';

CREATE TABLE IF NOT EXISTS `Discussions_Forum`
(
	`SiteId`                       int unsigned      NOT NULL COMMENT '主键，站点编号',
	`ForumId`                      smallint unsigned NOT NULL COMMENT '主键，论坛编号',
	`GroupId`                      smallint unsigned NOT NULL COMMENT '论坛组编号',
	`Name`                         varchar(50)       NOT NULL COMMENT '论坛名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Description`                  varchar(500)      NULL     COMMENT '描述文本' COLLATE 'utf8mb4_0900_ai_ci',
	`CoverPicturePath`             varchar(200)      NULL     COMMENT '封面路径' COLLATE 'ascii_general_ci',
	`Ordinal`                      smallint          NOT NULL COMMENT '排列顺序' DEFAULT 0,
	`IsPopular`                    tinyint(1)        NOT NULL COMMENT '是否热门版块' DEFAULT 0,
	`Approvable`                   tinyint(1)        NOT NULL COMMENT '发帖是否审核' DEFAULT 0,
	`Visibility`                   tinyint unsigned  NOT NULL COMMENT '可见范围' DEFAULT 2,
	`Accessibility`                tinyint unsigned  NOT NULL COMMENT '可访问性' DEFAULT 2,
	`TotalPosts`                   int unsigned      NOT NULL COMMENT '累计帖子总数' DEFAULT 0,
	`TotalThreads`                 int unsigned      NOT NULL COMMENT '累计主题总数' DEFAULT 0,
	`MostRecentThreadId`           bigint unsigned   NULL     COMMENT '最新主题编号',
	`MostRecentThreadTitle`        varchar(100)      NULL     COMMENT '最新主题标题' COLLATE 'utf8mb4_0900_ai_ci',
	`MostRecentThreadAuthorId`     int unsigned      NULL     COMMENT '最新主题作者编号',
	`MostRecentThreadAuthorName`   varchar(50)       NULL     COMMENT '最新主题作者名称' COLLATE 'utf8mb4_0900_ai_ci',
	`MostRecentThreadAuthorAvatar` varchar(100)      NULL     COMMENT '最新主题作者头像' COLLATE 'ascii_general_ci',
	`MostRecentThreadTime`         datetime          NULL     COMMENT '最新主题发布时间',
	`MostRecentPostId`             bigint unsigned   NULL     COMMENT '最后回帖编号',
	`MostRecentPostAuthorId`       int unsigned      NULL     COMMENT '最后回帖作者编号',
	`MostRecentPostAuthorName`     varchar(50)       NULL     COMMENT '最后回帖作者名称' COLLATE 'utf8mb4_0900_ai_ci',
	`MostRecentPostAuthorAvatar`   varchar(100)      NULL     COMMENT '最后回帖作者头像' COLLATE 'ascii_general_ci',
	`MostRecentPostTime`           datetime          NULL     COMMENT '最后回帖时间',
	`CreatorId`                    int unsigned      NOT NULL COMMENT '创建人编号',
	`CreatedTime`                  datetime          NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`SiteId`, `ForumId`)
) ENGINE=InnoDB COMMENT='论坛表';

CREATE TABLE IF NOT EXISTS `Discussions_ForumUser`
(
	`SiteId`      int unsigned      NOT NULL COMMENT '主键，站点编号',
	`ForumId`     smallint unsigned NOT NULL COMMENT '主键，论坛编号',
	`UserId`      int unsigned      NOT NULL COMMENT '主键，用户编号',
	`Permission`  tinyint unsigned  NOT NULL COMMENT '权限定义',
	`IsModerator` tinyint(1)        NOT NULL COMMENT '是否版主',
	PRIMARY KEY (`SiteId`, `ForumId`, `UserId`),
	CONSTRAINT `FK_ForumUser.ForumId` FOREIGN KEY (`SiteId`) REFERENCES `Discussions_Site` (`SiteId`) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='论坛用户表';

CREATE TABLE IF NOT EXISTS `Discussions_Post`
(
	`PostId`             bigint unsigned NOT NULL COMMENT '主键，帖子编号',
	`SiteId`             int unsigned    NOT NULL COMMENT '所属站点编号',
	`ThreadId`           bigint unsigned NOT NULL COMMENT '所属主题编号',
	`RefererId`          bigint unsigned NOT NULL COMMENT '回帖引用编号' DEFAULT 0,
	`Content`            varchar(500)    NOT NULL COMMENT '帖子内容' COLLATE 'utf8mb4_0900_ai_ci',
	`ContentType`        varchar(50)     NULL     COMMENT '内容类型' COLLATE 'ascii_general_ci',
	`Visible`            tinyint(1)      NOT NULL COMMENT '是否可见' DEFAULT 1,
	`Approved`           tinyint(1)      NOT NULL COMMENT '是否已审核' DEFAULT 1,
	`IsLocked`           tinyint(1)      NOT NULL COMMENT '是否已锁定' DEFAULT 0,
	`IsValued`           tinyint(1)      NOT NULL COMMENT '是否精华帖' DEFAULT 0,
	`TotalUpvotes`       int unsigned    NOT NULL COMMENT '累计点赞数' DEFAULT 0,
	`TotalDownvotes`     int unsigned    NOT NULL COMMENT '累计被踩数' DEFAULT 0,
	`VisitorAddress`     varchar(100)    NULL     COMMENT '访客地址' COLLATE 'utf8mb4_0900_ai_ci',
	`VisitorDescription` varchar(500)    NULL     COMMENT '访客描述' COLLATE 'utf8mb4_0900_ai_ci',
	`CreatorId`          int unsigned    NOT NULL COMMENT '发帖人编号',
	`CreatedTime`        datetime        NOT NULL COMMENT '发帖时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`PostId` DESC)
) ENGINE=InnoDB COMMENT='帖子表';

CREATE TABLE IF NOT EXISTS `Discussions_PostAttachment`
(
	`PostId`             bigint unsigned NOT NULL COMMENT '主键，帖子编号',
	`AttachmentId`       bigint unsigned NOT NULL COMMENT '主键，附件编号',
	`AttachmentFolderId` int unsigned    NOT NULL COMMENT '附件目录编号',
	`Ordinal`            smallint        NOT NULL COMMENT '排列顺序' DEFAULT 0,
	PRIMARY KEY (`PostId`, `AttachmentId`),
	INDEX `IX_Ordinal` (`PostId`, `AttachmentFolderId`, `Ordinal`)
) ENGINE=InnoDB COMMENT '帖子附件表';

CREATE TABLE IF NOT EXISTS `Discussions_PostVoting`
(
	`PostId`     bigint unsigned NOT NULL COMMENT '主键，帖子编号',
	`UserId`     int unsigned    NOT NULL COMMENT '主键，用户编号',
	`Value`      tinyint         NOT NULL COMMENT '投票数值',
	`Tiemstamp`  datetime        NOT NULL COMMENT '投票时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`PostId`, `UserId`)
) ENGINE=InnoDB COMMENT='帖子投票表';

CREATE TABLE IF NOT EXISTS `Discussions_History`
(
	`UserId`          int unsigned    NOT NULL COMMENT '主键，用户编号',
	`ThreadId`        bigint unsigned NOT NULL COMMENT '主键，主题编号',
	`ViewedCount`     int unsigned    NOT NULL COMMENT '浏览次数',
	`PostedCount`     int unsigned    NOT NULL COMMENT '发帖次数',
	`FirstViewedTime` datetime        NOT NULL COMMENT '首次浏览时间' DEFAULT CURRENT_TIMESTAMP,
	`FirstPostedTime` datetime        NULL     COMMENT '首次发帖时间',
	`LastViewedTime`  datetime        NOT NULL COMMENT '最后浏览时间' DEFAULT CURRENT_TIMESTAMP,
	`LastPostedTime`  datetime        NULL     COMMENT '最后发帖时间',
	PRIMARY KEY (`UserId`, `ThreadId` DESC)
) ENGINE=InnoDB COMMENT='访问历史表';

CREATE TABLE IF NOT EXISTS `Discussions_Thread`
(
	`ThreadId`                   bigint unsigned   NOT NULL COMMENT '主键，主题编号',
	`SiteId`                     int unsigned      NOT NULL COMMENT '所属站点编号',
	`ForumId`                    smallint unsigned NOT NULL COMMENT '所属论坛编号',
	`Title`                      varchar(50)       NOT NULL COMMENT '文章标题' COLLATE 'utf8mb4_0900_ai_ci',
	`Acronym`                    varchar(50)       NULL     COMMENT '标题缩写' COLLATE 'utf8mb4_0900_ai_ci',
	`Summary`                    varchar(500)      NOT NULL COMMENT '文章摘要' COLLATE 'utf8mb4_0900_ai_ci',
	`Tags`                       varchar(100)      NULL     COMMENT '标签集' COLLATE 'utf8mb4_0900_ai_ci',
	`PostId`                     bigint unsigned   NOT NULL COMMENT '主帖编号',
	`CoverPicturePath`           varchar(200)      NULL     COMMENT '封面路径' COLLATE 'ascii_general_ci',
	`LinkUrl`                    varchar(200)      NULL     COMMENT '主题链接' COLLATE 'utf8mb4_0900_ai_ci',
	`Visible`                    tinyint(1)        NOT NULL COMMENT '是否可见' DEFAULT 1,
	`Approved`                   tinyint(1)        NOT NULL COMMENT '是否审核' DEFAULT 1,
	`IsLocked`                   tinyint(1)        NOT NULL COMMENT '是否锁定' DEFAULT 0,
	`IsPinned`                   tinyint(1)        NOT NULL COMMENT '是否置顶' DEFAULT 0,
	`IsValued`                   tinyint(1)        NOT NULL COMMENT '是否精华帖' DEFAULT 0,
	`IsGlobal`                   tinyint(1)        NOT NULL COMMENT '是否全局贴' DEFAULT 0,
	`TotalViews`                 int unsigned      NOT NULL COMMENT '累计阅读数' DEFAULT 0,
	`TotalReplies`               int unsigned      NOT NULL COMMENT '累计回帖数' DEFAULT 0,
	`ApprovedTime`               datetime          NULL     COMMENT '审核通过时间',
	`ViewedTime`                 datetime          NULL     COMMENT '最后查看时间',
	`MostRecentPostId`           bigint unsigned   NULL     COMMENT '最后回帖编号',
	`MostRecentPostAuthorId`     int unsigned      NULL     COMMENT '最后回帖作者编号',
	`MostRecentPostAuthorName`   varchar(50)       NULL     COMMENT '最后回帖作者名称' COLLATE 'utf8mb4_0900_ai_ci',
	`MostRecentPostAuthorAvatar` varchar(100)      NULL     COMMENT '最后回帖作者头像' COLLATE 'ascii_general_ci',
	`MostRecentPostTime`         datetime          NULL     COMMENT '最后回帖时间',
	`CreatorId`                  int unsigned      NOT NULL COMMENT '作者编号',
	`CreatedTime`                datetime          NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`ThreadId` DESC)
) ENGINE=InnoDB COMMENT='主题表';

CREATE TABLE IF NOT EXISTS `Discussions_UserProfile`
(
	`UserId`                int unsigned     NOT NULL COMMENT '主键，用户编号',
	`SiteId`                int unsigned     NOT NULL COMMENT '所属站点编号',
	`Name`                  varchar(50)      NOT NULL COMMENT '用户名称' COLLATE 'utf8mb4_0900_ai_ci',
	`Nickname`              varchar(50)      NULL     COMMENT '用户昵称' COLLATE 'utf8mb4_0900_ai_ci',
	`Email`                 varchar(50)      NULL     COMMENT '邮箱地址' COLLATE 'ascii_general_ci',
	`Phone`                 varchar(50)      NULL     COMMENT '手机号码' COLLATE 'ascii_general_ci',
	`Avatar`                varchar(100)     NULL     COMMENT '用户头像' COLLATE 'ascii_general_ci',
	`PhotoPath`             varchar(200)     NULL     COMMENT '照片路径' COLLATE 'ascii_general_ci',
	`Gender`                tinyint unsigned NOT NULL COMMENT '用户性别' DEFAULT 0,
	`Grade`                 tinyint unsigned NOT NULL COMMENT '用户等级' DEFAULT 0,
	`TotalPosts`            int unsigned     NOT NULL COMMENT '累计回复总数' DEFAULT 0,
	`TotalThreads`          int unsigned     NOT NULL COMMENT '累计主题总数' DEFAULT 0,
	`MostRecentPostId`      bigint unsigned  NULL     COMMENT '最后回帖编号',
	`MostRecentPostTime`    datetime         NULL     COMMENT '最后回帖时间',
	`MostRecentThreadId`    bigint unsigned  NULL     COMMENT '最新主题编号',
	`MostRecentThreadTitle` varchar(100)     NULL     COMMENT '最新主题标题' COLLATE 'utf8mb4_0900_ai_ci',
	`MostRecentThreadTime`  datetime         NULL     COMMENT '最新主题时间',
	`Creation`              datetime         NOT NULL COMMENT '创建时间' DEFAULT CURRENT_TIMESTAMP,
	`Modification`          datetime         NULL     COMMENT '修改时间',
	`Description`           varchar(500)     NULL     COMMENT '描述信息' COLLATE 'utf8mb4_0900_ai_ci',
	PRIMARY KEY (`UserId`),
	UNIQUE INDEX `UX_User_Name` (`SiteId`, `Name`),
	UNIQUE INDEX `UX_User_Email` (`SiteId`, `Email`),
	UNIQUE INDEX `UX_User_Phone` (`SiteId`, `Phone`)
) ENGINE=InnoDB COMMENT='用户信息表';

CREATE TABLE IF NOT EXISTS `Discussions_UserMessage`
(
	`UserId`    int unsigned        NOT NULL COMMENT '主键，用户编号',
	`MessageId` bigint unsigned     NOT NULL COMMENT '主键，消息编号',
	`IsRead`    tinyint(1) unsigned NOT NULL COMMENT '是否已读',
	PRIMARY KEY (`UserId`, `MessageId`),
	CONSTRAINT `FK_UserMessage.UserId` FOREIGN KEY (`UserId`) REFERENCES `Discussions_UserProfile` (`UserId`) ON DELETE CASCADE,
	CONSTRAINT `FK_UserMessage.MessageId` FOREIGN KEY (`MessageId`) REFERENCES `Discussions_Message` (`MessageId`) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='用户消息表';

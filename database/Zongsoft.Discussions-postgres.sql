CREATE TABLE IF NOT EXISTS "Discussions_Feedback"
(
	"FeedbackId"  bigint       NOT NULL,
	"SiteId"      int          NOT NULL,
	"Kind"        smallint     NOT NULL,
	"Subject"     varchar(50)  NOT NULL COLLATE "C.utf8",
	"Content"     varchar(500) NOT NULL COLLATE "C.utf8",
	"ContentType" varchar(100) NULL     COLLATE "C",
	"ContactName" varchar(50)  NOT NULL COLLATE "C.utf8",
	"ContactText" varchar(50)  NOT NULL COLLATE "C.utf8",
	"CreatedTime" timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("FeedbackId")
);

COMMENT ON TABLE "Discussions_Feedback" IS '反馈表';
COMMENT ON COLUMN "Discussions_Feedback"."FeedbackId"  IS '主键，反馈编号';
COMMENT ON COLUMN "Discussions_Feedback"."SiteId"      IS '站点编号';
COMMENT ON COLUMN "Discussions_Feedback"."Kind"        IS '反馈种类';
COMMENT ON COLUMN "Discussions_Feedback"."Subject"     IS '反馈标题';
COMMENT ON COLUMN "Discussions_Feedback"."Content"     IS '反馈内容';
COMMENT ON COLUMN "Discussions_Feedback"."ContentType" IS '内容类型';
COMMENT ON COLUMN "Discussions_Feedback"."ContactName" IS '联系人名称';
COMMENT ON COLUMN "Discussions_Feedback"."ContactText" IS '联系人方式';
COMMENT ON COLUMN "Discussions_Feedback"."CreatedTime" IS '创建时间';


CREATE TABLE IF NOT EXISTS "Discussions_Message"
(
	"MessageId"   bigint       NOT NULL,
	"SiteId"      int          NOT NULL,
	"Subject"     varchar(50)  NOT NULL COLLATE "C.utf8",
	"Content"     varchar(500) NOT NULL COLLATE "C.utf8",
	"ContentType" varchar(100) NULL     COLLATE "C",
	"MessageType" varchar(50)  NULL     COLLATE "C",
	"Referer"     varchar(50)  NULL     COLLATE "C",
	"Tags"        varchar(100) NULL     COLLATE "C.utf8",
	"CreatorId"   int          NOT NULL,
	"CreatedTime" timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("MessageId")
);

COMMENT ON TABLE "Discussions_Message" IS '消息表';
COMMENT ON COLUMN "Discussions_Message"."MessageId"   IS '主键，消息编号';
COMMENT ON COLUMN "Discussions_Message"."SiteId"      IS '站点编号';
COMMENT ON COLUMN "Discussions_Message"."Subject"     IS '消息标题';
COMMENT ON COLUMN "Discussions_Message"."Content"     IS '消息内容';
COMMENT ON COLUMN "Discussions_Message"."ContentType" IS '内容类型';
COMMENT ON COLUMN "Discussions_Message"."MessageType" IS '消息类型';
COMMENT ON COLUMN "Discussions_Message"."Referer"     IS '消息来源';
COMMENT ON COLUMN "Discussions_Message"."Tags"        IS '标签集';
COMMENT ON COLUMN "Discussions_Message"."CreatorId"   IS '创建人编号';
COMMENT ON COLUMN "Discussions_Message"."CreatedTime" IS '创建时间';

CREATE TABLE IF NOT EXISTS "Discussions_Folder"
(
	"FolderId"     int          NOT NULL,
	"SiteId"       int          NOT NULL,
	"Name"         varchar(50)  NOT NULL COLLATE "C.utf8",
	"Acronym"      varchar(50)  NULL     COLLATE "C.utf8",
	"Icon"         varchar(50)  NULL     COLLATE "C",
	"Shareability" smallint     NOT NULL,
	"CreatorId"    int          NOT NULL,
	"CreatedTime"  timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Description"  varchar(500) NULL     COLLATE "C.utf8",
	PRIMARY KEY ("FolderId")
);

COMMENT ON TABLE "Discussions_Folder" IS '目录表';
COMMENT ON COLUMN "Discussions_Folder"."FolderId"     IS '主键，目录编号';
COMMENT ON COLUMN "Discussions_Folder"."SiteId"       IS '站点编号';
COMMENT ON COLUMN "Discussions_Folder"."Name"         IS '目录名称';
COMMENT ON COLUMN "Discussions_Folder"."Acronym"      IS '名称缩写';
COMMENT ON COLUMN "Discussions_Folder"."Icon"         IS '显示图标';
COMMENT ON COLUMN "Discussions_Folder"."Shareability" IS '可共享性';
COMMENT ON COLUMN "Discussions_Folder"."CreatorId"    IS '创建人编号';
COMMENT ON COLUMN "Discussions_Folder"."CreatedTime"  IS '创建时间';
COMMENT ON COLUMN "Discussions_Folder"."Description"  IS '描述文本';

CREATE TABLE IF NOT EXISTS "Discussions_FolderUser"
(
	"FolderId"   int       NOT NULL,
	"UserId"     int       NOT NULL,
	"Permission" smallint  NOT NULL,
	"Expiration" timestamp NULL,
	PRIMARY KEY ("FolderId", "UserId"),
	CONSTRAINT "FK_FolderUser.FolderId" FOREIGN KEY ("FolderId") REFERENCES "Discussions_Folder" ("FolderId") ON DELETE CASCADE
);

COMMENT ON TABLE "Discussions_FolderUser" IS '目录用户表';
COMMENT ON COLUMN "Discussions_FolderUser"."FolderId"   IS '主键，目录编号';
COMMENT ON COLUMN "Discussions_FolderUser"."UserId"     IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_FolderUser"."Permission" IS '权限定义';
COMMENT ON COLUMN "Discussions_FolderUser"."Expiration" IS '过期时间';

CREATE TABLE IF NOT EXISTS "Discussions_File"
(
	"FileId"      bigint       NOT NULL,
	"SiteId"      int          NOT NULL DEFAULT 0,
	"FolderId"    int          NOT NULL DEFAULT 0,
	"Name"        varchar(50)  NOT NULL COLLATE "C.utf8",
	"Acronym"     varchar(50)  NULL     COLLATE "C.utf8",
	"Path"        varchar(200) NOT NULL COLLATE "C.utf8",
	"Type"        varchar(100) NULL     COLLATE "C",
	"Size"        int          NOT NULL,
	"Tags"        varchar(100) NULL     COLLATE "C.utf8",
	"CreatorId"   int          NOT NULL,
	"CreatedTime" timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Description" varchar(500) NULL     COLLATE "C.utf8",
	PRIMARY KEY ("FileId")
);

COMMENT ON TABLE "Discussions_File" IS '文件（附件）表';
COMMENT ON COLUMN "Discussions_File"."FileId"      IS '主键，文件编号';
COMMENT ON COLUMN "Discussions_File"."SiteId"      IS '所属站点编号';
COMMENT ON COLUMN "Discussions_File"."FolderId"    IS '所属目录编号';
COMMENT ON COLUMN "Discussions_File"."Name"        IS '文件名称';
COMMENT ON COLUMN "Discussions_File"."Acronym"     IS '名称缩写';
COMMENT ON COLUMN "Discussions_File"."Path"        IS '文件路径';
COMMENT ON COLUMN "Discussions_File"."Type"        IS '文件类型';
COMMENT ON COLUMN "Discussions_File"."Size"        IS '文件大小';
COMMENT ON COLUMN "Discussions_File"."Tags"        IS '标签集';
COMMENT ON COLUMN "Discussions_File"."CreatorId"   IS '创建人编号';
COMMENT ON COLUMN "Discussions_File"."CreatedTime" IS '创建时间';
COMMENT ON COLUMN "Discussions_File"."Description" IS '描述文本';

CREATE TABLE IF NOT EXISTS "Discussions_Site"
(
	"SiteId"      int          NOT NULL,
	"SiteNo"      varchar(50)  NOT NULL COLLATE "C",
	"Name"        varchar(50)  NOT NULL COLLATE "C.utf8",
	"Host"        varchar(50)  NOT NULL COLLATE "C",
	"Icon"        varchar(100) NULL     COLLATE "C",
	"Domain"      varchar(50)  NOT NULL COLLATE "C",
	"Description" varchar(500) NULL     COLLATE "C.utf8",
	PRIMARY KEY ("SiteId")
);

CREATE UNIQUE INDEX IF NOT EXISTS "UX_Discussions_Site_SiteNo" ON "Discussions_Site" USING btree
  ("SiteNo" ASC);
CREATE INDEX IF NOT EXISTS "IX_Discussions_Site_Host" ON "Discussions_Site" USING btree
  ("Host" ASC);
CREATE INDEX IF NOT EXISTS "IX_Discussions_Site_Domain" ON "Discussions_Site" USING btree
  ("Domain" ASC);

COMMENT ON TABLE "Discussions_Site" IS '站点表';
COMMENT ON COLUMN "Discussions_Site"."SiteId"      IS '主键，站点编号';
COMMENT ON COLUMN "Discussions_Site"."SiteNo"      IS '站点代号';
COMMENT ON COLUMN "Discussions_Site"."Name"        IS '站点名称';
COMMENT ON COLUMN "Discussions_Site"."Host"        IS '站点域名';
COMMENT ON COLUMN "Discussions_Site"."Icon"        IS '显示图标';
COMMENT ON COLUMN "Discussions_Site"."Domain"      IS '所属领域';
COMMENT ON COLUMN "Discussions_Site"."Description" IS '描述文本';

CREATE TABLE IF NOT EXISTS "Discussions_SiteUser"
(
	"SiteId" int NOT NULL,
	"UserId" int NOT NULL,
	PRIMARY KEY ("SiteId", "UserId"),
	CONSTRAINT "FK_SiteUser.SiteId" FOREIGN KEY ("SiteId") REFERENCES "Discussions_Site" ("SiteId") ON DELETE CASCADE
);

COMMENT ON TABLE "Discussions_SiteUser" IS '站点用户表';
COMMENT ON COLUMN "Discussions_SiteUser"."SiteId" IS '主键，站点编号';
COMMENT ON COLUMN "Discussions_SiteUser"."UserId" IS '主键，用户编号';

CREATE TABLE IF NOT EXISTS "Discussions_ForumGroup"
(
	"SiteId"      int          NOT NULL,
	"GroupId"     smallint     NOT NULL,
	"Name"        varchar(50)  NOT NULL COLLATE "C.utf8",
	"Icon"        varchar(100) NULL     COLLATE "C",
	"Ordinal"     smallint     NOT NULL,
	"Description" varchar(500) NULL     COLLATE "C.utf8",
	PRIMARY KEY ("SiteId", "GroupId")
);

COMMENT ON TABLE "Discussions_ForumGroup" IS '论坛分组表';
COMMENT ON COLUMN "Discussions_ForumGroup"."SiteId"      IS '主键，站点编号';
COMMENT ON COLUMN "Discussions_ForumGroup"."GroupId"     IS '主键，分组编号';
COMMENT ON COLUMN "Discussions_ForumGroup"."Name"        IS '论坛组名';
COMMENT ON COLUMN "Discussions_ForumGroup"."Icon"        IS '显示图标';
COMMENT ON COLUMN "Discussions_ForumGroup"."Ordinal"     IS '排列顺序';
COMMENT ON COLUMN "Discussions_ForumGroup"."Description" IS '描述文本';

CREATE TABLE IF NOT EXISTS "Discussions_Forum"
(
	"SiteId"                       int               NOT NULL,
	"ForumId"                      smallint          NOT NULL,
	"GroupId"                      smallint          NOT NULL,
	"Name"                         varchar(50)       NOT NULL COLLATE "C.utf8",
	"Description"                  varchar(500)      NULL     COLLATE "C.utf8",
	"CoverPicturePath"             varchar(200)      NULL     COLLATE "C",
	"Ordinal"                      smallint          NOT NULL DEFAULT 0,
	"IsPopular"                    boolean           NOT NULL DEFAULT false,
	"Approvable"                   boolean           NOT NULL DEFAULT false,
	"Visibility"                   smallint          NOT NULL DEFAULT 2,
	"Accessibility"                smallint          NOT NULL DEFAULT 2,
	"TotalPosts"                   int               NOT NULL DEFAULT 0,
	"TotalThreads"                 int               NOT NULL DEFAULT 0,
	"MostRecentThreadId"           bigint            NULL,
	"MostRecentThreadTitle"        varchar(100)      NULL     COLLATE "C.utf8",
	"MostRecentThreadAuthorId"     int               NULL,
	"MostRecentThreadAuthorName"   varchar(50)       NULL     COLLATE "C.utf8",
	"MostRecentThreadAuthorAvatar" varchar(100)      NULL     COLLATE "C",
	"MostRecentThreadTime"         timestamp         NULL,
	"MostRecentPostId"             bigint            NULL,
	"MostRecentPostAuthorId"       int               NULL,
	"MostRecentPostAuthorName"     varchar(50)       NULL     COLLATE "C.utf8",
	"MostRecentPostAuthorAvatar"   varchar(100)      NULL     COLLATE "C",
	"MostRecentPostTime"           timestamp         NULL,
	"CreatorId"                    int               NOT NULL,
	"CreatedTime"                  timestamp         NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("SiteId", "ForumId")
);

COMMENT ON TABLE "Discussions_Forum" IS '论坛表';
COMMENT ON COLUMN "Discussions_Forum"."SiteId"                       IS '主键，站点编号';
COMMENT ON COLUMN "Discussions_Forum"."ForumId"                      IS '主键，论坛编号';
COMMENT ON COLUMN "Discussions_Forum"."GroupId"                      IS '分组编号';
COMMENT ON COLUMN "Discussions_Forum"."Name"                         IS '论坛名称';
COMMENT ON COLUMN "Discussions_Forum"."Description"                  IS '描述文本';
COMMENT ON COLUMN "Discussions_Forum"."CoverPicturePath"             IS '封面路径';
COMMENT ON COLUMN "Discussions_Forum"."Ordinal"                      IS '排列顺序';
COMMENT ON COLUMN "Discussions_Forum"."IsPopular"                    IS '是否热门版块';
COMMENT ON COLUMN "Discussions_Forum"."Approvable"                   IS '发帖是否审核';
COMMENT ON COLUMN "Discussions_Forum"."Visibility"                   IS '可见范围';
COMMENT ON COLUMN "Discussions_Forum"."Accessibility"                IS '可访问性';
COMMENT ON COLUMN "Discussions_Forum"."TotalPosts"                   IS '累计帖子总数';
COMMENT ON COLUMN "Discussions_Forum"."TotalThreads"                 IS '累计主题总数';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadId"           IS '最新主题编号';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadTitle"        IS '最新主题标题';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadAuthorId"     IS '最新主题作者编号';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadAuthorName"   IS '最新主题作者名称';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadAuthorAvatar" IS '最新主题作者头像';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentThreadTime"         IS '最新主题发布时间';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentPostId"             IS '最后回帖编号';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentPostAuthorId"       IS '最后回帖作者编号';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentPostAuthorName"     IS '最后回帖作者名称';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentPostAuthorAvatar"   IS '最后回帖作者头像';
COMMENT ON COLUMN "Discussions_Forum"."MostRecentPostTime"           IS '最后回帖时间';
COMMENT ON COLUMN "Discussions_Forum"."CreatorId"                    IS '创建人编号';
COMMENT ON COLUMN "Discussions_Forum"."CreatedTime"                  IS '创建时间';

CREATE TABLE IF NOT EXISTS "Discussions_ForumUser"
(
	"SiteId"      int      NOT NULL,
	"ForumId"     smallint NOT NULL,
	"UserId"      int      NOT NULL,
	"Permission"  smallint NOT NULL,
	"IsModerator" boolean  NOT NULL DEFAULT false,
	PRIMARY KEY ("SiteId", "ForumId", "UserId"),
	CONSTRAINT "FK_ForumUser.ForumId" FOREIGN KEY ("SiteId") REFERENCES "Discussions_Site" ("SiteId") ON DELETE CASCADE
);

COMMENT ON TABLE "Discussions_ForumUser" IS '论坛用户表';
COMMENT ON COLUMN "Discussions_ForumUser"."SiteId"      IS '主键，站点编号';
COMMENT ON COLUMN "Discussions_ForumUser"."ForumId"     IS '主键，论坛编号';
COMMENT ON COLUMN "Discussions_ForumUser"."UserId"      IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_ForumUser"."Permission"  IS '权限定义';
COMMENT ON COLUMN "Discussions_ForumUser"."IsModerator" IS '是否版主';

CREATE TABLE IF NOT EXISTS "Discussions_Post"
(
	"PostId"             bigint       NOT NULL,
	"SiteId"             int          NOT NULL,
	"ThreadId"           bigint       NOT NULL,
	"RefererId"          bigint       NOT NULL DEFAULT 0,
	"Content"            varchar(500) NOT NULL COLLATE "C.utf8",
	"ContentType"        varchar(50)  NULL     COLLATE "C",
	"Visible"            boolean      NOT NULL DEFAULT true,
	"Approved"           boolean      NOT NULL DEFAULT true,
	"IsLocked"           boolean      NOT NULL DEFAULT false,
	"IsValued"           boolean      NOT NULL DEFAULT false,
	"TotalUpvotes"       int          NOT NULL DEFAULT 0,
	"TotalDownvotes"     int          NOT NULL DEFAULT 0,
	"VisitorAddress"     varchar(100) NULL     COLLATE "C.utf8",
	"VisitorDescription" varchar(500) NULL     COLLATE "C.utf8",
	"CreatorId"          int          NOT NULL,
	"CreatedTime"        timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("PostId")
);

CREATE UNIQUE INDEX IF NOT EXISTS "UX_Discussions_Post_PostId" ON "Discussions_Post" USING btree
  ("PostId" DESC);

COMMENT ON TABLE "Discussions_Post" IS '帖子表';
COMMENT ON COLUMN "Discussions_Post"."PostId"             IS '主键，帖子编号';
COMMENT ON COLUMN "Discussions_Post"."SiteId"             IS '所属站点编号';
COMMENT ON COLUMN "Discussions_Post"."ThreadId"           IS '所属主题编号';
COMMENT ON COLUMN "Discussions_Post"."RefererId"          IS '回帖引用编号';
COMMENT ON COLUMN "Discussions_Post"."Content"            IS '帖子内容';
COMMENT ON COLUMN "Discussions_Post"."ContentType"        IS '内容类型';
COMMENT ON COLUMN "Discussions_Post"."Visible"            IS '是否可见';
COMMENT ON COLUMN "Discussions_Post"."Approved"           IS '是否已审核';
COMMENT ON COLUMN "Discussions_Post"."IsLocked"           IS '是否已锁定';
COMMENT ON COLUMN "Discussions_Post"."IsValued"           IS '是否精华帖';
COMMENT ON COLUMN "Discussions_Post"."TotalUpvotes"       IS '累计点赞数';
COMMENT ON COLUMN "Discussions_Post"."TotalDownvotes"     IS '累计被踩数';
COMMENT ON COLUMN "Discussions_Post"."VisitorAddress"     IS '访客地址';
COMMENT ON COLUMN "Discussions_Post"."VisitorDescription" IS '访客描述';
COMMENT ON COLUMN "Discussions_Post"."CreatorId"          IS '发帖人编号';
COMMENT ON COLUMN "Discussions_Post"."CreatedTime"        IS '发帖时间';

CREATE TABLE IF NOT EXISTS "Discussions_PostAttachment"
(
	"PostId"             bigint   NOT NULL,
	"AttachmentId"       bigint   NOT NULL,
	"AttachmentFolderId" int      NOT NULL,
	"Ordinal"            smallint NOT NULL DEFAULT 0,
	PRIMARY KEY ("PostId", "AttachmentId")
);

CREATE INDEX IF NOT EXISTS "IX_Discussions_PostAttachment_Ordinal" ON "Discussions_PostAttachment" USING btree
  ("PostId", "AttachmentFolderId", "Ordinal" ASC);

COMMENT ON TABLE "Discussions_PostAttachment" IS '帖子附件表';
COMMENT ON COLUMN "Discussions_PostAttachment"."PostId"             IS '主键，帖子编号';
COMMENT ON COLUMN "Discussions_PostAttachment"."AttachmentId"       IS '主键，附件编号';
COMMENT ON COLUMN "Discussions_PostAttachment"."AttachmentFolderId" IS '附件目录编号';
COMMENT ON COLUMN "Discussions_PostAttachment"."Ordinal"            IS '排列顺序';

CREATE TABLE IF NOT EXISTS "Discussions_PostVoting"
(
	"PostId"     bigint    NOT NULL,
	"UserId"     int       NOT NULL,
	"Value"      smallint  NOT NULL,
	"Tiemstamp"  timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("PostId", "UserId")
);

COMMENT ON TABLE "Discussions_PostVoting" IS '帖子投票表';
COMMENT ON COLUMN "Discussions_PostVoting"."PostId"    IS '主键，帖子编号';
COMMENT ON COLUMN "Discussions_PostVoting"."UserId"    IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_PostVoting"."Value"     IS '投票数值';
COMMENT ON COLUMN "Discussions_PostVoting"."Tiemstamp" IS '投票时间';

CREATE TABLE IF NOT EXISTS "Discussions_History"
(
	"UserId"          int       NOT NULL,
	"ThreadId"        bigint    NOT NULL,
	"ViewedCount"     int       NOT NULL,
	"PostedCount"     int       NOT NULL,
	"FirstViewedTime" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"FirstPostedTime" timestamp NULL,
	"LastViewedTime"  timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"LastPostedTime"  timestamp NULL,
	PRIMARY KEY ("UserId", "ThreadId")
);

COMMENT ON TABLE "Discussions_History" IS '访问历史表';
COMMENT ON COLUMN "Discussions_History"."UserId"          IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_History"."ThreadId"        IS '主键，主题编号';
COMMENT ON COLUMN "Discussions_History"."ViewedCount"     IS '浏览次数';
COMMENT ON COLUMN "Discussions_History"."PostedCount"     IS '发帖次数';
COMMENT ON COLUMN "Discussions_History"."FirstViewedTime" IS '首次浏览时间';
COMMENT ON COLUMN "Discussions_History"."FirstPostedTime" IS '首次发帖时间';
COMMENT ON COLUMN "Discussions_History"."LastViewedTime"  IS '最后浏览时间';
COMMENT ON COLUMN "Discussions_History"."LastPostedTime"  IS '最后发帖时间';

CREATE TABLE IF NOT EXISTS "Discussions_Thread"
(
	"ThreadId"                   bigint       NOT NULL,
	"SiteId"                     int          NOT NULL,
	"ForumId"                    smallint     NOT NULL,
	"Title"                      varchar(50)  NOT NULL COLLATE "C.utf8",
	"Acronym"                    varchar(50)  NULL     COLLATE "C.utf8",
	"Summary"                    varchar(500) NOT NULL COLLATE "C.utf8",
	"Tags"                       varchar(100) NULL     COLLATE "C.utf8",
	"PostId"                     bigint       NOT NULL,
	"CoverPicturePath"           varchar(200) NULL     COLLATE "C",
	"LinkUrl"                    varchar(200) NULL     COLLATE "C.utf8",
	"Visible"                    boolean      NOT NULL DEFAULT true,
	"Approved"                   boolean      NOT NULL DEFAULT true,
	"IsLocked"                   boolean      NOT NULL DEFAULT false,
	"IsPinned"                   boolean      NOT NULL DEFAULT false,
	"IsValued"                   boolean      NOT NULL DEFAULT false,
	"IsGlobal"                   boolean      NOT NULL DEFAULT false,
	"TotalViews"                 int          NOT NULL DEFAULT 0,
	"TotalReplies"               int          NOT NULL DEFAULT 0,
	"ApprovedTime"               timestamp    NULL,
	"ViewedTime"                 timestamp    NULL,
	"MostRecentPostId"           bigint       NULL,
	"MostRecentPostAuthorId"     int          NULL,
	"MostRecentPostAuthorName"   varchar(50)  NULL     COLLATE "C.utf8",
	"MostRecentPostAuthorAvatar" varchar(100) NULL     COLLATE "C",
	"MostRecentPostTime"         timestamp    NULL,
	"CreatorId"                  int          NOT NULL,
	"CreatedTime"                timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY ("ThreadId")
);

CREATE UNIQUE INDEX IF NOT EXISTS "UX_Discussions_Thread_ThreadId" ON "Discussions_Thread" USING btree
  ("ThreadId" DESC);

COMMENT ON TABLE "Discussions_Thread" IS '主题表';
COMMENT ON COLUMN "Discussions_Thread"."ThreadId"                   IS '主键，主题编号';
COMMENT ON COLUMN "Discussions_Thread"."SiteId"                     IS '所属站点编号';
COMMENT ON COLUMN "Discussions_Thread"."ForumId"                    IS '所属论坛编号';
COMMENT ON COLUMN "Discussions_Thread"."Title"                      IS '文章标题';
COMMENT ON COLUMN "Discussions_Thread"."Acronym"                    IS '标题缩写';
COMMENT ON COLUMN "Discussions_Thread"."Summary"                    IS '文章摘要';
COMMENT ON COLUMN "Discussions_Thread"."Tags"                       IS '标签集';
COMMENT ON COLUMN "Discussions_Thread"."PostId"                     IS '主帖编号';
COMMENT ON COLUMN "Discussions_Thread"."CoverPicturePath"           IS '封面路径';
COMMENT ON COLUMN "Discussions_Thread"."LinkUrl"                    IS '主题链接';
COMMENT ON COLUMN "Discussions_Thread"."Visible"                    IS '是否可见';
COMMENT ON COLUMN "Discussions_Thread"."Approved"                   IS '是否审核';
COMMENT ON COLUMN "Discussions_Thread"."IsLocked"                   IS '是否锁定';
COMMENT ON COLUMN "Discussions_Thread"."IsPinned"                   IS '是否置顶';
COMMENT ON COLUMN "Discussions_Thread"."IsValued"                   IS '是否精华帖';
COMMENT ON COLUMN "Discussions_Thread"."IsGlobal"                   IS '是否全局贴';
COMMENT ON COLUMN "Discussions_Thread"."TotalViews"                 IS '累计阅读数';
COMMENT ON COLUMN "Discussions_Thread"."TotalReplies"               IS '累计回帖数';
COMMENT ON COLUMN "Discussions_Thread"."ApprovedTime"               IS '审核通过时间';
COMMENT ON COLUMN "Discussions_Thread"."ViewedTime"                 IS '最后查看时间';
COMMENT ON COLUMN "Discussions_Thread"."MostRecentPostId"           IS '最后回帖编号';
COMMENT ON COLUMN "Discussions_Thread"."MostRecentPostAuthorId"     IS '最后回帖作者编号';
COMMENT ON COLUMN "Discussions_Thread"."MostRecentPostAuthorName"   IS '最后回帖作者名称';
COMMENT ON COLUMN "Discussions_Thread"."MostRecentPostAuthorAvatar" IS '最后回帖作者头像';
COMMENT ON COLUMN "Discussions_Thread"."MostRecentPostTime"         IS '最后回帖时间';
COMMENT ON COLUMN "Discussions_Thread"."CreatorId"                  IS '作者编号';
COMMENT ON COLUMN "Discussions_Thread"."CreatedTime"                IS '创建时间';

CREATE TABLE IF NOT EXISTS "Discussions_UserProfile"
(
	"UserId"                int          NOT NULL,
	"SiteId"                int          NOT NULL,
	"Name"                  varchar(50)  NOT NULL COLLATE "C.utf8",
	"Nickname"              varchar(50)  NULL     COLLATE "C.utf8",
	"Email"                 varchar(50)  NULL     COLLATE "C",
	"Phone"                 varchar(50)  NULL     COLLATE "C",
	"Avatar"                varchar(100) NULL     COLLATE "C",
	"Gender"                smallint     NOT NULL DEFAULT 0,
	"Grade"                 smallint     NOT NULL DEFAULT 0,
	"TotalPosts"            int          NOT NULL DEFAULT 0,
	"TotalThreads"          int          NOT NULL DEFAULT 0,
	"MostRecentPostId"      bigint       NULL,
	"MostRecentPostTime"    timestamp    NULL,
	"MostRecentThreadId"    bigint       NULL,
	"MostRecentThreadTitle" varchar(100) NULL     COLLATE "C.utf8",
	"MostRecentThreadTime"  timestamp    NULL,
	"Creation"              timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Modification"          timestamp    NULL,
	"Description"           varchar(500) NULL     COLLATE "C.utf8",
	PRIMARY KEY ("UserId")
);

CREATE INDEX IF NOT EXISTS "UX_Discussions_UserProfile_Name" ON "Discussions_UserProfile" USING btree
  ("SiteId", "Name" ASC);
CREATE INDEX IF NOT EXISTS "UX_Discussions_UserProfile_Email" ON "Discussions_UserProfile" USING btree
  ("SiteId", "Email" ASC);
CREATE INDEX IF NOT EXISTS "UX_Discussions_UserProfile_Phone" ON "Discussions_UserProfile" USING btree
  ("SiteId", "Phone" ASC);

COMMENT ON TABLE "Discussions_UserProfile" IS '用户信息表';
COMMENT ON COLUMN "Discussions_UserProfile"."UserId"                IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_UserProfile"."SiteId"                IS '所属站点编号';
COMMENT ON COLUMN "Discussions_UserProfile"."Name"                  IS '用户名称';
COMMENT ON COLUMN "Discussions_UserProfile"."Nickname"              IS '用户昵称';
COMMENT ON COLUMN "Discussions_UserProfile"."Email"                 IS '邮箱地址';
COMMENT ON COLUMN "Discussions_UserProfile"."Phone"                 IS '手机号码';
COMMENT ON COLUMN "Discussions_UserProfile"."Avatar"                IS '用户头像';
COMMENT ON COLUMN "Discussions_UserProfile"."Gender"                IS '用户性别';
COMMENT ON COLUMN "Discussions_UserProfile"."TotalPosts"            IS '累计回复总数';
COMMENT ON COLUMN "Discussions_UserProfile"."TotalThreads"          IS '累计主题总数';
COMMENT ON COLUMN "Discussions_UserProfile"."MostRecentPostId"      IS '最后回帖编号';
COMMENT ON COLUMN "Discussions_UserProfile"."MostRecentPostTime"    IS '最后回帖时间';
COMMENT ON COLUMN "Discussions_UserProfile"."MostRecentThreadId"    IS '最新主题编号';
COMMENT ON COLUMN "Discussions_UserProfile"."MostRecentThreadTitle" IS '最新主题标题';
COMMENT ON COLUMN "Discussions_UserProfile"."MostRecentThreadTime"  IS '最新主题时间';
COMMENT ON COLUMN "Discussions_UserProfile"."Creation"              IS '创建时间';
COMMENT ON COLUMN "Discussions_UserProfile"."Modification"          IS '修改时间';
COMMENT ON COLUMN "Discussions_UserProfile"."Description"           IS '描述信息';

CREATE TABLE IF NOT EXISTS "Discussions_UserMessage"
(
	"UserId"    int     NOT NULL,
	"MessageId" bigint  NOT NULL,
	"IsRead"    boolean NOT NULL DEFAULT false,
	PRIMARY KEY ("UserId", "MessageId"),
	CONSTRAINT "FK_UserMessage.UserId" FOREIGN KEY ("UserId") REFERENCES "Discussions_UserProfile" ("UserId") ON DELETE CASCADE,
	CONSTRAINT "FK_UserMessage.MessageId" FOREIGN KEY ("MessageId") REFERENCES "Discussions_Message" ("MessageId") ON DELETE CASCADE
);

COMMENT ON TABLE "Discussions_UserMessage" IS '用户消息表';
COMMENT ON COLUMN "Discussions_UserMessage"."UserId"    IS '主键，用户编号';
COMMENT ON COLUMN "Discussions_UserMessage"."MessageId" IS '主键，消息编号';
COMMENT ON COLUMN "Discussions_UserMessage"."IsRead"    IS '是否已读';

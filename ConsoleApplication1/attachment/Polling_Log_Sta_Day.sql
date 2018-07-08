/*
Navicat SQL Server Data Transfer

Source Server         : msslocal
Source Server Version : 105000
Source Host           : .:1433
Source Database       : ydupdate
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 105000
File Encoding         : 65001

Date: 2018-07-08 15:07:24
*/


-- ----------------------------
-- Table structure for Polling_Log_Sta_Day
-- ----------------------------
DROP TABLE [dbo].[Polling_Log_Sta_Day]
GO
CREATE TABLE [dbo].[Polling_Log_Sta_Day] (
[id] int NOT NULL IDENTITY(1,1) ,
[year] int NULL ,
[month] int NULL ,
[day] int NULL ,
[mscid] int NULL ,
[occurtime] datetime NULL ,
[H0] decimal(20,2) NULL ,
[Hn0] decimal(20,2) NULL ,
[H1] decimal(20,2) NULL ,
[Hn1] decimal(20,2) NULL ,
[H2] decimal(20,2) NULL ,
[Hn2] decimal(20,2) NULL ,
[H3] decimal(20,2) NULL ,
[Hn3] decimal(20,2) NULL ,
[H4] decimal(20,2) NULL ,
[Hn4] decimal(20,2) NULL ,
[H5] decimal(20,2) NULL ,
[Hn5] decimal(20,2) NULL ,
[H6] decimal(20,2) NULL ,
[Hn6] decimal(20,2) NULL ,
[H7] decimal(20,2) NULL ,
[Hn7] decimal(20,2) NULL ,
[H8] decimal(20,2) NULL ,
[Hn8] decimal(20,2) NULL ,
[H9] decimal(20,2) NULL ,
[Hn9] decimal(20,2) NULL ,
[H10] decimal(20,2) NULL ,
[Hn10] decimal(20,2) NULL ,
[H11] decimal(20,2) NULL ,
[Hn11] decimal(20,2) NULL ,
[H12] decimal(20,2) NULL ,
[Hn12] decimal(20,2) NULL ,
[H13] decimal(20,2) NULL ,
[Hn13] decimal(20,2) NULL ,
[H14] decimal(20,2) NULL ,
[Hn14] decimal(20,2) NULL ,
[H15] decimal(20,2) NULL ,
[Hn15] decimal(20,2) NULL ,
[H16] decimal(20,2) NULL ,
[Hn16] decimal(20,2) NULL ,
[H17] decimal(20,2) NULL ,
[Hn17] decimal(20,2) NULL ,
[H18] decimal(20,2) NULL ,
[Hn18] decimal(20,2) NULL ,
[H19] decimal(20,2) NULL ,
[Hn19] decimal(20,2) NULL ,
[H20] decimal(20,2) NULL ,
[Hn20] decimal(20,2) NULL ,
[H21] decimal(20,2) NULL ,
[Hn21] decimal(20,2) NULL ,
[H22] decimal(20,2) NULL ,
[Hn22] decimal(20,2) NULL ,
[H23] decimal(20,2) NULL ,
[Hn23] decimal(20,2) NULL ,
[fcid] int NULL ,
[haveData] tinyint NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[Polling_Log_Sta_Day]', RESEED, 25780)
GO

-- ----------------------------
-- Indexes structure for table Polling_Log_Sta_Day
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table Polling_Log_Sta_Day
-- ----------------------------
ALTER TABLE [dbo].[Polling_Log_Sta_Day] ADD PRIMARY KEY ([id])
GO

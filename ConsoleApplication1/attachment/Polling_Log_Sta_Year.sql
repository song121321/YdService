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

Date: 2018-07-08 15:07:45
*/


-- ----------------------------
-- Table structure for Polling_Log_Sta_Year
-- ----------------------------
DROP TABLE [dbo].[Polling_Log_Sta_Year]
GO
CREATE TABLE [dbo].[Polling_Log_Sta_Year] (
[id] int NOT NULL IDENTITY(1,1) ,
[year] int NULL ,
[mscid] int NULL ,
[occurtime] datetime NULL ,
[M1] decimal(20,2) NULL ,
[Mn1] decimal(20,2) NULL ,
[M2] decimal(20,2) NULL ,
[Mn2] decimal(20,2) NULL ,
[M3] decimal(20,2) NULL ,
[Mn3] decimal(20,2) NULL ,
[M4] decimal(20,2) NULL ,
[Mn4] decimal(20,2) NULL ,
[M5] decimal(20,2) NULL ,
[Mn5] decimal(20,2) NULL ,
[M6] decimal(20,2) NULL ,
[Mn6] decimal(20,2) NULL ,
[M7] decimal(20,2) NULL ,
[Mn7] decimal(20,2) NULL ,
[M8] decimal(20,2) NULL ,
[Mn8] decimal(20,2) NULL ,
[M9] decimal(20,2) NULL ,
[Mn9] decimal(20,2) NULL ,
[M10] decimal(20,2) NULL ,
[Mn10] decimal(20,2) NULL ,
[M11] decimal(20,2) NULL ,
[Mn11] decimal(20,2) NULL ,
[M12] decimal(20,2) NULL ,
[Mn12] decimal(20,2) NULL ,
[fcid] int NULL ,
[haveData] tinyint NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[Polling_Log_Sta_Year]', RESEED, 819)
GO

-- ----------------------------
-- Indexes structure for table Polling_Log_Sta_Year
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table Polling_Log_Sta_Year
-- ----------------------------
ALTER TABLE [dbo].[Polling_Log_Sta_Year] ADD PRIMARY KEY ([id])
GO

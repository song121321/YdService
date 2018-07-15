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

Date: 2018-07-08 15:07:37
*/


-- ----------------------------
-- Table structure for Polling_Log_Sta_Month
-- ----------------------------
DROP TABLE [dbo].[Polling_Log_Sta_Month]
GO
CREATE TABLE [dbo].[Polling_Log_Sta_Month] (
[id] int NOT NULL IDENTITY(1,1) ,
[year] int NULL ,
[month] int NULL ,
[mscid] int NULL ,
[occurtime] datetime NULL ,
[D1] decimal(20,2) NULL ,
[Dn1] decimal(20,2) NULL ,
[D2] decimal(20,2) NULL ,
[Dn2] decimal(20,2) NULL ,
[D3] decimal(20,2) NULL ,
[Dn3] decimal(20,2) NULL ,
[D4] decimal(20,2) NULL ,
[Dn4] decimal(20,2) NULL ,
[D5] decimal(20,2) NULL ,
[Dn5] decimal(20,2) NULL ,
[D6] decimal(20,2) NULL ,
[Dn6] decimal(20,2) NULL ,
[D7] decimal(20,2) NULL ,
[Dn7] decimal(20,2) NULL ,
[D8] decimal(20,2) NULL ,
[Dn8] decimal(20,2) NULL ,
[D9] decimal(20,2) NULL ,
[Dn9] decimal(20,2) NULL ,
[D10] decimal(20,2) NULL ,
[Dn10] decimal(20,2) NULL ,
[D11] decimal(20,2) NULL ,
[Dn11] decimal(20,2) NULL ,
[D12] decimal(20,2) NULL ,
[Dn12] decimal(20,2) NULL ,
[D13] decimal(20,2) NULL ,
[Dn13] decimal(20,2) NULL ,
[D14] decimal(20,2) NULL ,
[Dn14] decimal(20,2) NULL ,
[D15] decimal(20,2) NULL ,
[Dn15] decimal(20,2) NULL ,
[D16] decimal(20,2) NULL ,
[Dn16] decimal(20,2) NULL ,
[D17] decimal(20,2) NULL ,
[Dn17] decimal(20,2) NULL ,
[D18] decimal(20,2) NULL ,
[Dn18] decimal(20,2) NULL ,
[D19] decimal(20,2) NULL ,
[Dn19] decimal(20,2) NULL ,
[D20] decimal(20,2) NULL ,
[Dn20] decimal(20,2) NULL ,
[D21] decimal(20,2) NULL ,
[Dn21] decimal(20,2) NULL ,
[D22] decimal(20,2) NULL ,
[Dn22] decimal(20,2) NULL ,
[D23] decimal(20,2) NULL ,
[Dn23] decimal(20,2) NULL ,
[D24] decimal(20,2) NULL ,
[Dn24] decimal(20,2) NULL ,
[D25] decimal(20,2) NULL ,
[Dn25] decimal(20,2) NULL ,
[D26] decimal(20,2) NULL ,
[Dn26] decimal(20,2) NULL ,
[D27] decimal(20,2) NULL ,
[Dn27] decimal(20,2) NULL ,
[D28] decimal(20,2) NULL ,
[Dn28] decimal(20,2) NULL ,
[D29] decimal(20,2) NULL ,
[Dn29] decimal(20,2) NULL ,
[D30] decimal(20,2) NULL ,
[Dn30] decimal(20,2) NULL ,
[D31] decimal(20,2) NULL ,
[Dn31] decimal(20,2) NULL ,
[fcid] int NULL ,
[haveData] tinyint NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[Polling_Log_Sta_Month]', RESEED, 12622)
GO

-- ----------------------------
-- Indexes structure for table Polling_Log_Sta_Month
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table Polling_Log_Sta_Month
-- ----------------------------
ALTER TABLE [dbo].[Polling_Log_Sta_Month] ADD PRIMARY KEY ([id])
GO

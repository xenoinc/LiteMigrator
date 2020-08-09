# Lite Migrator for mobile devices

<img align="right" width="90" height="90" src="https://raw.githubusercontent.com/xenoinc/SQLiteMigrator/master/docs/logo.png">Lite Migrator (_SQLite Migrator_) is a tiny SQL migration framework for .NET inspired by [Fluent Migrator](https://github.com/fluentmigrator/fluentmigrator). This framework was built for use with Xamarin 🐒 projects, so it needs to be quick, simple and reliable when managing databases

Sponsored by [Xeno Innovations](https://xenoinc.com), this project was made with nerd-love.

**_This project is a Work in Progress_**

## Use it in your project
⚠️ This is in beta - _NuGet package coming soon_, so we need your help to make this project better!

Currently, we recommend you add this to your project using Git's submodule so you always get the latest.

## Sample Migration


## How to Contribute
1. Fork on GitHub
2. Create a branch
3. Code (_and add tests)
4. Create a Pull Request (_PR_) on GitHub
   1. Target the ``develop`` branch and we'll get it merged up to ``master``
   2. Target the ``master`` branch for hotfixes
5. Get the PR merged
6. Welcome to our contributors' list!

## Known Limitations

### SQLite Triggers.
Because we parse on the semicolon, having another semicolon inside a statement currently is not supported.

As an example, notice the ``update`` statement inside of the ``create trigger``:
```sql
CREATE TABLE SalesOrder (
  [Id] VARCHAR(36) PRIMARY KEY NOT NULL,
  [OrderGuid] VARCHAR(36) NOT NULL,
  [CreatedByUserId] VARCHAR(36) NOT NULL,
  [CreatedDttm] DATETIME NOT NULL,
  [RegisterId] VARCHAR(36) NOT NULL,
  [OrderStatusId] INT NOT NULL,
  [CustomerId] VARCHAR(36) NULL,
  [ModifiedDttm] DATETIME NULL,
  [ModifiedSalesPrice] FLOAT NULL,
  [ModifiedByUserId] VARCHAR(36) NULL,
  [SyncGuid] VARCHAR(36) NULL,
  [SyncUpdatedDttm] DATETIME NULL,
  [SyncIsDeleted] BIT NULL DEFAULT(0),
  CONSTRAINT FK_SalesOrder_OrderStatusType_OrderStatusId FOREIGN KEY (OrderStatusId) REFERENCES OrderStatusType(Id),
  CONSTRAINT FK_SalesOrder_User_ModifiedByUserId FOREIGN KEY (ModifiedByUserId) REFERENCES UserInfo(Id)
);

DROP TRIGGER IF EXISTS TRG_UserInfo_Id_AutoGenerateGuid;

CREATE TRIGGER TRG_UserInfo_Id_AutoGenerateGuid
AFTER INSERT ON UserInfo
FOR EACH ROW
BEGIN
 UPDATE UserInfo SET Id =
   (SELECT HEX( RANDOMBLOB(4)) || '-' || HEX( RANDOMBLOB(2))
           || '-' || '4' || SUBSTR(HEX( RANDOMBLOB(2)), 2) || '-'
           || SUBSTR('AB89', 1 + (ABS(RANDOM()) % 4) , 1)  ||
           SUBSTR(HEX(RANDOMBLOB(2)), 2) || '-' || HEX(RANDOMBLOB(6)) ) WHERE RowId = NEW.RowId;
END;

DROP TRIGGER IF EXISTS TRG_Sync_Id_AutoGenerateGuid;

CREATE TRIGGER TRG_Sync_Id_AutoGenerateGuid
AFTER INSERT ON Sync
FOR EACH ROW
-- WHEN (NEW.Id IS NULL)
BEGIN
 UPDATE Sync SET Id =
   (SELECT HEX( RANDOMBLOB(4)) || '-' || HEX( RANDOMBLOB(2))
           || '-' || '4' || SUBSTR(HEX( RANDOMBLOB(2)), 2) || '-'
           || SUBSTR('AB89', 1 + (ABS(RANDOM()) % 4) , 1)  ||
           SUBSTR(HEX(RANDOMBLOB(2)), 2) || '-' || HEX(RANDOMBLOB(6)) ) WHERE RowId = NEW.RowId;
END;
```

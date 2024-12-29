
# Known Limitations

No system is perfect. This framework only gets better when contributors like yourself enhance it.

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

### Recommendations

Enhance the parser engine. Review other methodologies such as:

* https://github.com/andialbrecht/sqlparse

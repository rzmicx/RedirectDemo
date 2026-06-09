;WITH TargetData AS (
    SELECT 
        Id,
        ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM NamaTable
),
RandomNumbers AS (
    SELECT 
        CAST(number AS VARCHAR(5)) AS RandomCode,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn
    FROM (
        SELECT TOP (90000)
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) + 9999 AS number
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
    ) x
)
UPDATE t
SET t.RandomCode = r.RandomCode
FROM NamaTable t
JOIN TargetData d ON t.Id = d.Id
JOIN RandomNumbers r ON d.rn = r.rn;

ALTER TABLE NamaTable
ADD CONSTRAINT UQ_NamaTable_RandomCode UNIQUE (RandomCode);



CREATE SEQUENCE dbo.RandomCodeSeq
AS INT
START WITH 1
INCREMENT BY 1
MINVALUE 1
MAXVALUE 90000
NO CYCLE;

CREATE TRIGGER TR_YourTable_SetRandomCode
ON YourTable
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t
    SET RandomCode = CAST(
        ((NEXT VALUE FOR dbo.RandomCodeSeq * 7919 + 12345) % 90000) + 10000
        AS VARCHAR(5)
    )
    FROM YourTable t
    INNER JOIN inserted i ON t.Id = i.Id
    WHERE t.RandomCode IS NULL;
END;

BEGIN TRAN;

-- 3. Isi data lama pakai rumus yang sama dengan sequence
;WITH TargetData AS (
    SELECT 
        Id,
        ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM NamaTable
)
UPDATE t
SET t.RandomCode = CAST(
    ((d.rn * 7919 + 12345) % 900000) + 100000
    AS VARCHAR(6)
)
FROM NamaTable t
JOIN TargetData d ON t.Id = d.Id;

-- 4. Tambahkan UNIQUE constraint
ALTER TABLE NamaTable
ADD CONSTRAINT UQ_NamaTable_RandomCode UNIQUE (RandomCode);

-- 5. Buat sequence mulai dari data berikutnya
DECLARE @CurrentCount INT = (SELECT COUNT(*) FROM NamaTable);
DECLARE @Sql NVARCHAR(MAX);

SET @Sql = '
CREATE SEQUENCE dbo.RandomCodeSeq
AS INT
START WITH ' + CAST(@CurrentCount + 1 AS VARCHAR(20)) + '
INCREMENT BY 1
MINVALUE 1
MAXVALUE 900000
NO CYCLE;
';

EXEC sp_executesql @Sql;

COMMIT;

CREATE TRIGGER TR_NamaTable_SetRandomCode
ON NamaTable
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE t
    SET RandomCode = CAST(
        ((NEXT VALUE FOR dbo.RandomCodeSeq * 7919 + 12345) % 900000) + 100000
        AS VARCHAR(6)
    )
    FROM NamaTable t
    INNER JOIN inserted i ON t.Id = i.Id
    WHERE t.RandomCode IS NULL;
END;

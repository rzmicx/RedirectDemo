BEGIN TRAN;

-- 1. Pastikan jumlah data tidak lebih dari 90.000
IF (SELECT COUNT(*) FROM NamaTable) > 90000
BEGIN
    ROLLBACK;
    THROW 50001, 'Jumlah data lebih dari 90.000, kode 5 digit tidak cukup.', 1;
END;

-- 2. Isi data lama pakai rumus yang sama dengan sequence
;WITH TargetData AS (
    SELECT 
        Id,
        ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM NamaTable
)
UPDATE t
SET t.RandomCode = CAST(
    ((d.rn * 7919 + 12345) % 90000) + 10000
    AS VARCHAR(5)
)
FROM NamaTable t
JOIN TargetData d ON t.Id = d.Id;

-- 3. Tambahkan UNIQUE constraint
ALTER TABLE NamaTable
ADD CONSTRAINT UQ_NamaTable_RandomCode UNIQUE (RandomCode);

-- 4. Ambil jumlah data lama
DECLARE @CurrentCount INT = (SELECT COUNT(*) FROM NamaTable);
DECLARE @Sql NVARCHAR(MAX);

-- 5. Buat sequence mulai dari data berikutnya
SET @Sql = '
CREATE SEQUENCE dbo.RandomCodeSeq
AS INT
START WITH ' + CAST(@CurrentCount + 1 AS VARCHAR(20)) + '
INCREMENT BY 1
MINVALUE 1
MAXVALUE 90000
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
        ((NEXT VALUE FOR dbo.RandomCodeSeq * 7919 + 12345) % 90000) + 10000
        AS VARCHAR(5)
    )
    FROM NamaTable t
    INNER JOIN inserted i ON t.Id = i.Id
    WHERE t.RandomCode IS NULL;
END;

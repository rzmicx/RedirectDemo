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

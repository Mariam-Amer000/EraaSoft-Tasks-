
SELECT * FROM Doctors

SELECT *
FROM Patients
ORDER BY Patient_age 


SELECT *
FROM Patients
ORDER BY URnumber
OFFSET 4 ROWS
FETCH NEXT 10 ROWS ONLY


SELECT TOP (5) *
FROM Doctors


SELECT DISTINCT address
FROM Patients


SELECT *
FROM Patients
WHERE Patient_age = 25


SELECT *
FROM Patients
WHERE email IS NULL


SELECT *
FROM Doctors
WHERE yearOfExperience > 5
AND specialty = 'Cardiology'


SELECT *
FROM Doctors
WHERE specialty IN ('Dermatology','Oncology')


SELECT *
FROM Patients
WHERE Patient_age BETWEEN 18 AND 30;

SELECT *
FROM Doctors
WHERE Doc_name LIKE 'Dr.%';


SELECT
    Doc_name AS DoctorName,
    Doc_email AS DoctorEmail
FROM Doctors


SELECT
    DPD.*,
    P.patient_name
FROM DoctorPatientDrug DPD
INNER JOIN Patients P
ON DPD.URnumber = P.URnumber


SELECT
    address,
    COUNT(*) AS PatientCount
FROM Patients
GROUP BY address


SELECT
    address,
    COUNT(*) AS PatientCount
FROM Patients
GROUP BY address
HAVING COUNT(*) > 3


SELECT
    address,
    Patient_age,
    COUNT(*) AS PatientCount
FROM Patients
GROUP BY GROUPING SETS
(
    (address),
    (Patient_age),
    (address, Patient_age)
);


SELECT
    address,
    Patient_age,
    COUNT(*) AS PatientCount
FROM Patients
GROUP BY CUBE(address, Patient_age);


SELECT
    address,
    COUNT(*) AS PatientCount
FROM Patients
GROUP BY ROLLUP(address);


SELECT *
FROM Patients P
WHERE EXISTS
(
    SELECT 1
    FROM DoctorPatientDrug DPD
    WHERE DPD.URnumber = P.URnumber
);


SELECT Doc_name AS Name
FROM Doctors

UNION

SELECT patient_name
FROM Patients


WITH PatientDoctor AS
(
    SELECT
        P.patient_name,
        D.Doc_name
    FROM Patients P
    INNER JOIN Doctors D
    ON P.Doc_id = D.Doc_id
)
SELECT *
FROM PatientDoctor


INSERT INTO Doctors
VALUES
(
    1,
    'Dr. Ahmed',
    'ahmed@gmail.com',
    12,
    'Cardiology',
    01012345678
)


INSERT INTO Patients
VALUES
(101,'Ali','Cairo','ali@gmail.com',22,123456,01111111111,1),
(102,'Sara','Giza','sara@gmail.com',25,NULL,01111111112,1),
(103,'Omar','Alex','omar@gmail.com',30,987654,01111111113,1)


UPDATE Doctors
SET phone = 01099999999
WHERE Doc_id = 1


UPDATE P
SET address = 'Cairo'
FROM Patients P
INNER JOIN DoctorPatientDrug DPD
ON P.URnumber = DPD.URnumber
INNER JOIN Doctors D
ON DPD.Doc_id = D.Doc_id
WHERE D.Doc_name = 'Dr. Ahmed'


DELETE FROM Patients
WHERE URnumber = 101;


BEGIN TRANSACTION

INSERT INTO Doctors
VALUES
(
    2,
    'Dr. Mona',
    'mona@gmail.com',
    8,
    'Dermatology',
    01022222222
)

INSERT INTO Patients
VALUES
(
    201,
    'Yousef',
    'Mansoura',
    'yousef@gmail.com',
    24,
    NULL,
    01122222222,
    2
)

COMMIT


CREATE VIEW PatientDoctorView
AS
SELECT
    P.patient_name,
    P.address,
    P.Patient_age,
    D.Doc_name,
    D.specialty
FROM Patients P
INNER JOIN Doctors D
ON P.Doc_id = D.Doc_id


CREATE INDEX IX_PatientPhone
ON Patients(phone)


BACKUP DATABASE HEALTH
TO DISK = 'C:\Backup\HEALTH.bak'
WITH INIT
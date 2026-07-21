CREATE DATABASE CompanyDB
GO 

USE CompanyDB
GO 

CREATE SCHEMA Sales
GO

CREATE TABLE Sales.employees
(
	emp_id INT PRIMARY KEY,
	first_name VARCHAR(35),
	last_name VARCHAR(35),
	salary decimal
)
GO

ALTER TABLE [Sales].[employees]
ADD hire_date DATE
GO

SELECT * 
FROM [Sales].[employees]
GO

SELECT first_name ,last_name 
FROM [Sales].[employees]
GO

SELECT first_name +' '+last_name AS full_name
FROM [Sales].[employees]
GO

SELECT AVG(salary)
FROM [Sales].[employees]
GO

SELECT *
FROM [Sales].[employees]
WHERE salary > 50000
GO

SELECT *
FROM [Sales].[employees]
wHERE hire_date >= '2020-01-01' AND hire_date <= '2020-12-31'
GO

SELECT *
FROM [Sales].[employees]
wHERE last_name like 's%'
GO

SELECT top 10 *
FROM [Sales].[employees]
order by salary desc
GO

SELECT *
FROM [Sales].[employees]
WHERE salary > 40000 and salary <60000
GO

SELECT *
FROM [Sales].[employees]
WHERE first_name  like '%man%' or last_name  like '%man%'
GO


SELECT *
FROM [Sales].[employees]
WHERE hire_date is null
GO

SELECT *
FROM [Sales].[employees]
WHERE salary in (40000,45000,50000)
GO

SELECT *
FROM [Sales].[employees]
wHERE hire_date > '2020-01-01' AND hire_date < '2021-01-01'
GO


SELECT *
FROM [Sales].[employees]
order by salary desc
GO

SELECT top 5 *
FROM [Sales].[employees]
order by last_name desc
GO

SELECT  *
FROM [Sales].[employees]
where salary>55000 and YEAR(hire_date)='2020'
GO

SELECT  *
FROM [Sales].[employees]
where first_name in ('john','jane')
GO

SELECT  *
FROM [Sales].[employees]
where salary<=55000 and hire_date> '2022-01-01'
GO

SELECT  *
FROM [Sales].[employees]
where salary> (select AVG(salary) from  [Sales].[employees])
GO 

SELECT  *
FROM [Sales].[employees]
ORDER BY salary DESC
OFFSET 2 ROWS
FETCH NEXT 5 ROWS ONLY
GO 

SELECT  *
FROM [Sales].[employees]
where hire_date > '2021-01-01'
order by first_name,last_name
GO 

SELECT  *
FROM [Sales].[employees]
where salary>50000  and last_name like '^A%'
GO 

SELECT  *
FROM [Sales].[employees]
where salary is not null
GO 


SELECT  *
FROM [Sales].[employees]
where salary>45000 and first_name like '%[ei]%' or   last_name like '%[ei]%'
GO 
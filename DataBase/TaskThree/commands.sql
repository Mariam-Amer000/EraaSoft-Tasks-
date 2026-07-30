CREATE DATABASE Health
GO 
USE Health
GO


CREATE TABLE Doctors
(
  Doc_id INT NOT NULL,
  Doc_name VARCHAR(50) NOT NULL,
  Doc_email VARCHAR(50) NOT NULL,
  yearOfExperience INT NOT NULL,
  specialty VARCHAR(50) NOT NULL,
  phone VARCHAR(11) NOT NULL,
  PRIMARY KEY (Doc_id)
);

CREATE TABLE Patients
(
  URnumber INT NOT NULL,
  patient_name VARCHAR(50) NOT NULL,
  address VARCHAR(50) NOT NULL,
  email VARCHAR(50) NOT NULL,
  Patient_age INT NOT NULL,
  medcareCardNumber INT,
  phone VARCHAR(11) NOT NULL,
  Doc_id INT NOT NULL,
  PRIMARY KEY (URnumber),
  FOREIGN KEY (Doc_id) REFERENCES Doctors(Doc_id)
);

CREATE TABLE Companys
(
  name VARCHAR(50) NOT NULL,
  address VARCHAR(50) NOT NULL,
  phone VARCHAR(11) NOT NULL,
  PRIMARY KEY (name)
);

CREATE TABLE Drugs
(
  strength INT NOT NULL,
  trade_name VARCHAR(50) NOT NULL,
  name VARCHAR(50) NOT NULL,
  PRIMARY KEY (strength, trade_name),
  FOREIGN KEY (name) REFERENCES companys(name)
);

CREATE TABLE DoctorPatientDrug
(
  date DATE NOT NULL,
  quantity INT NOT NULL,
  URnumber INT NOT NULL,
  Doc_id INT NOT NULL,
  strength INT NOT NULL,
  trade_name VARCHAR(50) NOT NULL,
  PRIMARY KEY (date, URnumber, Doc_id, strength, trade_name),
  FOREIGN KEY (URnumber) REFERENCES Patients(URnumber),
  FOREIGN KEY (Doc_id) REFERENCES Doctors(Doc_id),
  FOREIGN KEY (strength, trade_name) REFERENCES Drugs(strength, trade_name)
);
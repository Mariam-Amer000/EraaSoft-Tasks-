CREATE DATABASE movieDB
GO

USE movieDB
GO

CREATE TABLE actors
(
	act_id INT PRIMARY KEY,
	act_fname CHAR(20),
	act_lname CHAR(20),
	act_gender CHAR(1)
)
GO

CREATE TABLE director
(
	dir_id INT PRIMARY KEY,
	dir_fname CHAR(20),
	dir_lname CHAR(20),
)
GO

CREATE TABLE movie
(
	mov_id INT PRIMARY KEY,
	mov_title char(50),
	mov_year int,
	mov_time int,
	mov_lang char(50),
	mov_dt_rel date,
	mov_rel_country char(5)
)
GO

CREATE TABLE movie_direction
(
	dir_id int,
	mov_id int
	FOREIGN KEY (dir_id) REFERENCES [dbo].[director](dir_id),
	FOREIGN KEY (mov_id) REFERENCES [dbo].[movie](mov_id)
)
GO

CREATE TABLE movie_cast
(
	act_id int,
	mov_id int,
	role char(30)

	FOREIGN KEY (act_id) REFERENCES [dbo].[actors](act_id),
	FOREIGN KEY (mov_id) REFERENCES [dbo].[movie] (mov_id)
)
GO

CREATE TABLE reviewer 
(
	rev_id int PRIMARY KEY,
	rev_name char(30)
)
GO

CREATE TABLE genres
(
	gen_id int PRIMARY KEY,
	gen_title char(20)
)
GO 

CREATE TABLE movie_genres 
(
	mov_id int,
	gen_id int
	FOREIGN KEY (mov_id) REFERENCES [dbo].[movie](mov_id),
	FOREIGN KEY (gen_id) REFERENCES [dbo].[genres](gen_id)
)
GO

CREATE TABLE rating 
(
	mov_id int,
	rev_id int,
	rev_stars int,
	num_o_rating int

	FOREIGN KEY (mov_id) REFERENCES [dbo].[movie](mov_id),
	FOREIGN KEY (rev_id) REFERENCES [dbo].[reviewer] (rev_id)
)
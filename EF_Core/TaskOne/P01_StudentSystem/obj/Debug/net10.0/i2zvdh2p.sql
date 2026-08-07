IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Courses] (
    [CourseId] int NOT NULL IDENTITY,
    [Name] nvarchar(80) NOT NULL,
    [Description] nvarchar(max) NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([CourseId])
);

CREATE TABLE [Students] (
    [StudentId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [PhoneNumber] varchar(10) NULL,
    [RegisteredOn] datetime2 NOT NULL,
    [Birthday] date NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId])
);

CREATE TABLE [Resources] (
    [ResourceId] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Url] varchar(max) NOT NULL,
    [ResourceType] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Resources] PRIMARY KEY ([ResourceId]),
    CONSTRAINT [FK_Resources_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE
);

CREATE TABLE [Homeworks] (
    [HomeworkId] int NOT NULL IDENTITY,
    [Content] varchar(max) NOT NULL,
    [ContentType] int NOT NULL,
    [SubmissionTime] datetime2 NOT NULL,
    [StudentId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Homeworks] PRIMARY KEY ([HomeworkId]),
    CONSTRAINT [FK_Homeworks_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Homeworks_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE CASCADE
);

CREATE TABLE [StudentCourses] (
    [StudentId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_StudentCourses] PRIMARY KEY ([StudentId], [CourseId]),
    CONSTRAINT [FK_StudentCourses_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([CourseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentCourses_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE CASCADE
);

CREATE INDEX [IX_Homeworks_CourseId] ON [Homeworks] ([CourseId]);

CREATE INDEX [IX_Homeworks_StudentId] ON [Homeworks] ([StudentId]);

CREATE INDEX [IX_Resources_CourseId] ON [Resources] ([CourseId]);

CREATE INDEX [IX_StudentCourses_CourseId] ON [StudentCourses] ([CourseId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260803140737_InitialCreate', N'10.0.10');

COMMIT;
GO


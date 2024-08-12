

CREATE TABLE [dbo].[messages] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [firstname]  VARCHAR (100) NOT NULL,
    [lastname]   VARCHAR (100) NOT NULL,
    [email]      VARCHAR (150) NOT NULL,
    [phone]      VARCHAR (20)  NOT NULL,
    [subject]    VARCHAR (255) NOT NULL,
    [message]    TEXT          NOT NULL,
    [created_at] DATETIME      NOT NULL
);




CREATE TABLE [dbo].[Seeds] (
    [id]               INT             IDENTITY (1, 1) NOT NULL,
    [name]             VARCHAR (100)   NOT NULL,
    [type]             VARCHAR (255)   NOT NULL,
    [subtype]          VARCHAR (20)    NULL,
    [height]           INT             NULL,
    [germination_days] INT             NULL,
    [seed_depth]       NVARCHAR (10)   NULL,
    [plant_spacing]    INT             NULL,
    [sun_requirement]  VARCHAR (255)   NOT NULL,
    [season]           VARCHAR (100)   NOT NULL,
    [image_filename]   VARCHAR (255)   NULL,
    [created_at]       DATETIME        NOT NULL,
    
);



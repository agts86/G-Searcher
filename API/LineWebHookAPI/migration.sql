CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "ErrorLog" (
    "Id" uuid NOT NULL,
    "Contents" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ErrorLog" PRIMARY KEY ("Id")
);

CREATE TABLE "GourmetLocationLog" (
    "Id" uuid NOT NULL,
    "Lat" double precision NOT NULL,
    "Lng" double precision NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_GourmetLocationLog" PRIMARY KEY ("Id")
);

CREATE TABLE "GourmetWordLog" (
    "Id" uuid NOT NULL,
    "Text" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_GourmetWordLog" PRIMARY KEY ("Id")
);

CREATE TABLE "JobLog" (
    "Id" uuid NOT NULL,
    "IsSuccess" boolean NOT NULL,
    "Contents" text,
    "Info" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_JobLog" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260208051306_InitialMigration', '10.0.0');

COMMIT;


IF DB_ID('FinanceTracker_NoAuth') IS NULL
BEGIN
    CREATE DATABASE FinanceTracker_NoAuth;
END
GO

IF DB_ID('FinanceTracker_Auth') IS NULL
BEGIN
    CREATE DATABASE FinanceTracker_Auth;
END
GO

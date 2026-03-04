USE [URLShortenerDb];

DECLARE @AboutContent NVARCHAR(MAX) = N'
URL Shortener algorithm description

1) Validation and normalization
- Input URL is checked for null/empty value.
- The URL is trimmed to remove leading/trailing spaces.
- URLs are unique in the system (LongURL has unique index).

2) Duplicate prevention
- Before creating a new record, the system searches by normalized LongURL.
- If URL already exists, operation is rejected with a conflict error.

3) Short code generation
- A short code of fixed length 6 is generated.
- Allowed symbols: 0-9, a-z, A-Z.
- Code is generated randomly.

4) Uniqueness of short code
- For each generated code, database uniqueness is checked.
- If a collision occurs, generation is retried up to 20 attempts.
- If unique code cannot be produced, operation fails with conflict error.

5) Persistence
- New record fields:
  * Id (GUID)
  * LongURL
  * ShortCode
  * CreatedAt
  * CreatedById
- ShortCode and LongURL are unique at database level.

6) Redirect behavior
- Redirect endpoint receives short code.
- Long URL is resolved by ShortCode.
- Client gets HTTP redirect to original URL.
- If code is absent, not found error is returned.

7) Authorization rules
- Anonymous users: can view table, cannot add/delete, cannot open details.
- Authenticated user: can add URL, view details, delete only own records.
- Admin: can add URL, view details, delete any record.

8) About content rules
- About record is auto-created on first read/update if missing.
- Only Admin can update About content.
';

IF NOT EXISTS (SELECT 1 FROM [Abouts])
BEGIN
    INSERT INTO [Abouts] ([Id], [Content])
    VALUES (NEWID(), @AboutContent);
END
ELSE
BEGIN
    UPDATE TOP (1) [Abouts]
    SET [Content] = @AboutContent;
END
GO

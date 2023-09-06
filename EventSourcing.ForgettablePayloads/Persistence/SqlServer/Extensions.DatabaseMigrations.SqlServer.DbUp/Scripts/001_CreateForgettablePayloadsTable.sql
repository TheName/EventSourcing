CREATE TABLE [EventStream.ForgettablePayloads] (
    EventStreamId                           UNIQUEIDENTIFIER        NOT NULL,
    EventStreamEntryId                      UNIQUEIDENTIFIER        NOT NULL,
    PayloadId                               UNIQUEIDENTIFIER        NOT NULL,
    PayloadState                            VARCHAR(128)            NOT NULL,
    PayloadCreationTime                     DATETIMEOFFSET          NOT NULL,
    PayloadLastModifiedTime                 DATETIMEOFFSET          NOT NULL,
    PayloadSequence                         BIGINT                  NOT NULL,
    PayloadContent                          VARCHAR(MAX)            NOT NULL,
    PayloadContentSerializationFormat       VARCHAR(MAX)            NOT NULL,
    PayloadTypeIdentifier                   VARCHAR(MAX)            NOT NULL,
    PayloadTypeIdentifierFormat             VARCHAR(MAX)            NOT NULL,
    PRIMARY KEY (PayloadId)        
);

CREATE INDEX IDX_EventStreamId ON [EventStream.ForgettablePayloads] (EventStreamId);
CREATE INDEX IDX_EventStreamId_EventStreamEntryId ON [EventStream.ForgettablePayloads] (EventStreamId, EventStreamEntryId);
CREATE INDEX IDX_PayloadState ON [EventStream.ForgettablePayloads] (PayloadState);
CREATE INDEX IDX_EventStreamId_EventStreamEntryId_PayloadId_PayloadSequence ON [EventStream.ForgettablePayloads] (EventStreamId, EventStreamEntryId, PayloadId, PayloadSequence);
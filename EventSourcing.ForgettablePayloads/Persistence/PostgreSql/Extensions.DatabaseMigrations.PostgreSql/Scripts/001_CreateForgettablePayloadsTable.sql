CREATE TABLE "EventStream.ForgettablePayloads" (
    EventStreamId                           UUID        NOT NULL,
    EventStreamEntryId                      UUID        NOT NULL,
    PayloadId                               UUID        NOT NULL,
    PayloadState                            VARCHAR     NOT NULL,
    PayloadCreationTime                     TIMESTAMP   NOT NULL,
    PayloadCreationTimeNanoSeconds          BIGINT      NOT NULL,
    PayloadLastModifiedTime                 TIMESTAMP   NOT NULL,
    PayloadLastModifiedTimeNanoSeconds      BIGINT      NOT NULL,
    PayloadSequence                         BIGINT      NOT NULL,
    PayloadContent                          VARCHAR     NOT NULL,
    PayloadContentSerializationFormat       VARCHAR     NOT NULL,
    PayloadTypeIdentifier                   VARCHAR     NOT NULL,
    PayloadTypeIdentifierFormat             VARCHAR     NOT NULL,
    PRIMARY KEY (PayloadId)        
);

CREATE INDEX IDX_EventStreamId ON "EventStream.ForgettablePayloads" (EventStreamId);
CREATE INDEX IDX_EventStreamId_EventStreamEntryId ON "EventStream.ForgettablePayloads" (EventStreamId, EventStreamEntryId);
CREATE INDEX IDX_PayloadState ON "EventStream.ForgettablePayloads" (PayloadState);
CREATE INDEX IDX_EventStreamId_EventStreamEntryId_PayloadId_PayloadSequence ON "EventStream.ForgettablePayloads" (EventStreamId, EventStreamEntryId, PayloadId, PayloadSequence);
# YPractika

приложение починки орг.техники


для работы требуется MS SQL SERVER с данными таблицами
```

CREATE TABLE Users (
    userId INT PRIMARY KEY IDENTITY,
    login NVARCHAR(255) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    role NVARCHAR(50) NOT NULL,
    email NVARCHAR(255),
    fullName NVARCHAR(255)
);

CREATE TABLE Requests (
    requestId INT PRIMARY KEY IDENTITY,
    DateAdded DATETIME NOT NULL,
    DateEnded DATETIME,
    nameEquip NVARCHAR(255) NOT NULL,
    faultType NVARCHAR(255) NOT NULL,
    clientId INT,
    status NVARCHAR(50) NOT NULL,
    FOREIGN KEY (clientId) REFERENCES Users(userId)
);

CREATE TABLE Comments (
    commentId INT PRIMARY KEY IDENTITY,
    requestId INT,
    workerId INT,
    textComment NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (requestId) REFERENCES Requests(requestId),
    FOREIGN KEY (workerId) REFERENCES Users(userId)
);
```


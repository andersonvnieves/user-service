Feature: User Authentication

Scenario: User successfully authenticates with valid credentials
    Given a user with email "spec@test.com" and password "Password123!" exists
    When the user submits the login request with email "spec@test.com" and password "Password123!"
    Then the authentication should be successful
    And the response should contain an access token

Scenario: Authentication fails with invalid credentials
    Given a user with email "spec@test.com" and password "Password123!" exists
    When the user submits the login request with email "spec@test.com" and wrong password "WrongPassword!"
    Then the authentication should fail

Scenario: Authentication fails for non-existing user with generic error
    Given no user exists with email "unknown@test.com"
    When the user submits the login request with email "unknown@test.com" and password "AnyPassword123!"
    Then the authentication should fail
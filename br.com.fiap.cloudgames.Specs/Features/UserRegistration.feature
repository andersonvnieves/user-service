Feature: User Registration

Scenario: User successfully registers
    Given a user with first name "First", last name "Last", email "spec@test.com" and password "Password123!"
    When the user submits the registration request
    Then the account should be created successfully
    And the response should contain the user id
    
Scenario: Registration fails when email is not unique
    Given a user with first name "First", last name "Last", email "spec@test.com" and password "Password123!"
    And a user with email "spec@test.com" already exists
    When the user submits the registration request
    Then the registration should fail
    And an error "Email already in use" should be returned
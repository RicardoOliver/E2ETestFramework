Feature: User Login
    As a user
    I want to be able to login to the application
    So that I can access my account

Background:
    Given I am on the login page

@smoke @login
Scenario: Successful login with valid credentials
    When I enter valid username and password
    And I click the login button
    Then I should be redirected to the dashboard
    And I should see a welcome message

@negative @login
Scenario: Login with invalid credentials
    When I enter invalid username and password
    And I click the login button
    Then I should see an error message
    And I should remain on the login page

@negative @login
Scenario Outline: Login with various invalid credentials
    When I enter username "<username>" and password "<password>"
    And I click the login button
    Then I should see an error message "<error_message>"
    
    Examples:
    | username | password | error_message |
    | invalid  | invalid  | Your username is invalid! |
    | admin    | wrong    | Your username is invalid! |
    | empty    | empty    | Your username is invalid! |

@security @login
Scenario: Account lockout simulation after multiple failed attempts
    When I enter invalid credentials 3 times
    Then my account should be locked
    And I should see a lockout message

@accessibility @login
Scenario: Login page accessibility
    Then the login page should be accessible
    And all form elements should have proper labels
    And the page should support keyboard navigation

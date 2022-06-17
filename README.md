# RedRiftExApiGame

Task:
https://docs.google.com/document/d/18AseZN1CIfjpTrVxXOD4zrFPVZgHRNCCoiXdhHj2oJ0/edit

Tech stack for the project:
- ASP.NET Core Web Api project as base project type
- PostgreSQL as DB 
- EF Core and migrations for DB handling
- NUnit and Moq for Unit tests
- Heroku for hosting the solution
- Docker, Github & Github action for CI/CD
- Swagger for localhost API testing

# API endpoints:

## POST /api/create_lobby
Creates lobby with single host player by player name.
Returns basic information about the lobby, such as Id and players joined.

Use: POST "{\"playerName\": \"player-name\"}"
  
Example:

curl -X 'POST' \
  'https://redrift-exapi-game.herokuapp.com/api/create_lobby' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "playerName": "Jason Woorhis"
}'

Response:

{
  "id": "6203e4dc-4dcf-42e4-9c78-6887d7d1f456",
  "state": "Lobby",
  "currentGame": {
    "gameId": "6203e4dc-4dcf-42e4-9c78-6887d7d1f456",
    "playerHost": {
      "name": "Jason Woorhis",
      "playerHealth": 10
    },
    "playerGuest": null
  }
}

## GET /api/game-state
Checks state of the game by id. Queries active game from the server or ended games from the db.
  
Use: GET /api/game-state?gameid=game-id
  
Example:
  
curl -X 'GET' \
  'https://redrift-exapi-game.herokuapp.com/api/game-state?gameId=6203e4dc-4dcf-42e4-9c78-6887d7d1f456' \
  -H 'accept: text/plain'

Response:

{"id":"6203e4dc-4dcf-42e4-9c78-6887d7d1f456","state":"GameOver","currentGame":{"gameId":"6203e4dc-4dcf-42e4-9c78-6887d7d1f456","playerHost":{"name":"Jason Woorhis","playerHealth":0},"playerGuest":{"name":"valentine","playerHealth":2}}}

## POST /api/join-lobby
Tries to join player to the selected lobby by providing lobby id.
Returns Success if player successfully joined.
If lobby is full (already contains two players) returns LobbyIsFull.
If app can't find active lobby id returns Error.

Use: POST "{\"playerName\": \"player-name\", \"lobbyId\":\"lobby-id\"}"
  
Example:
  
curl -X 'POST' \
  'https://redrift-exapi-game.herokuapp.com/api/join-lobby' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "playerName": "valentine",
  "lobbyId": "6203e4dc-4dcf-42e4-9c78-6887d7d1f456"
}'

Response:

{
  "status": "Success",
  "gameId": "6203e4dc-4dcf-42e4-9c78-6887d7d1f456"
}

or

{
  "status": "LobbyIsFull",
  "gameId": "6203e4dc-4dcf-42e4-9c78-6887d7d1f456"
}

or

{
  "status": "Error",
  "gameId": "wrongID"
}

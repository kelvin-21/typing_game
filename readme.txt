Chan Ka Chun (1155094058)
IERG3080
Project Part (I) Documentation



This program uses model-view separation for decoupling model and view.


In the model it implements classes "Word" and "GameModel".

In the class "Word",
	- It particularly serves for counting number of letters correctly typed.
	
In the class "GameModel",
	- It is a singleton class that servers for the logic of the whole game.
	- It contains all the words that are pending to spawn. A queue stores them all.
	- It also contains the logic to calculate score and health whenever a word is typed or touches the bottom.
	
	
In the view, there are a class "SuperButton" and the MainWindow.

In the class "SuperButton",
	- It is the connection between the button (GUI) and the "Word" in model.
	- It contains the logic for the Word (in fact the button)
		-> button falling
		-> button goes back to the origin
		-> button touches the bottom

In the MainWindow,
	- It contains all the EventHandlers, including the Dispatcher Timers.
	
	
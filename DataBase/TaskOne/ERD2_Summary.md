# Entities

##### Airline

* ID
* name
* address
* name\_of\_Contact\_person
* telephone numbers

##### Employee

* id
* name
* address
* birthday

  * day
  * month
  * year
* gender
* position with company
* qualifications

##### Aircraft

* id
* capacity
* model

##### Route

* id
* origin
* destination
* distance
* classification

##### crew

* major pilot
* assistant pilot
* two hostesses

##### transaction

* id
* date
* description
* amount of money









# Relations

* Employee work Airline (M:1) T:P
* Airline own Aircraft (1:M) => P:T
* Route assignee Aircraft  (M:M) => T:T

  * NumberOfPassengers
  * PricePerPassenger
  * DepartureDateTime
  * ArrivalDateTime
  * &#x20;TravelTime
* Aircraft has Crew ()=> (1:1) => T:T
* Airline keepInfo Transaction (1:M) P:T


# Lab

https://127.0.0.1:2113/web/index.html#/streams

stream
> teststream 

event-types
> user-added
> user-updated


# ACL 
netsh http add urlacl url=https://+:2113/ user=DESKTOP-HSBK2QC\Hemma
netsh http show urlacl
netsh http delete urlacl url=https://+:2113/ 



# UI adding event: 
'--enable-atom-pub-over-http ' must be added in command.

https://127.0.0.1:2113/web/index.html#/streams/teststream/addEvent

```json
{
    "name": "jimi",
    "linkedin": "https://www.linkedin.com/in/jimi-friis-b729155/"
}
```


--enable-atom-pub-over-http 
EventStore.ClusterNode.exe --db ./db --log ./logs --config=config.yaml --enable-atom-pub-over-http 

# enable projections 
--run-projections
https://eventstore.com/docs/server/command-line-arguments/index.html#projections-options
EventStore.ClusterNode.exe --run-projections=all --start-standard-projections=true
To disable them again, run:
EventStore.ClusterNode.exe --run-projections=none


EventStore.ClusterNode.exe --db ./db --log ./logs --config=config.yaml --enable-atom-pub-over-http --run-projections=all --start-standard-projections=true


# add projections 
https://eventstore.com/docs/projections/user-defined-projections/index.html

```js
options({
    resultStreamName: "my_demo_projection_result"
})

fromStream('teststream')
.when({
    $init:function(){
        return {
            name: 'not set',
            nrOftimesChanged : 0
        }
    },
    "user-added": function(state, event){
        state.name = event.data.name;
		state.linkedin = event.data.linkedin;
		
    },
	"user-updated": function(state, event){
	    state.nrOftimesChanged += 1;
	    //here, loop over keys and values to know if they are updated. 
	    //foreach..  state[propertyName] = event.data[propertyName]
        state.name = event.data.name ? event.data.name : state.name;
		state.linkedin = event.data.linkedin ? event.data.linkedin: state.linkedin;		
    }
	
})
.outputState()
```


## projection looping all data props and adding to state 
```js
options({
    resultStreamName: "my_demo_projection_result"
})

fromStream('teststream')
.when({
    $init:function(){
        return {
            //name: 'not set',
            nrOftimesChanged : 0
        }
    },
    "user-added": function(state, event){
        //state.name = event.data.name;
		//state.linkedin = event.data.linkedin;
		
		//only loop over the 'data' prop as this is where new data is added.
		for (const [key, value] of Object.entries(event.data)) {
          //console.log(`logging-keys ${key}: ${value}`);
          state[key] = value;
        }
		
    },
	"user-updated": function(state, event){
	    state.nrOftimesChanged += 1;
	    //here, loop over keys and values to know if they are updated. 
	    //foreach..  state[propertyName] = event.data[propertyName]
        //state.name = event.data.name ? event.data.name : state.name;
		//state.linkedin = event.data.linkedin ? event.data.linkedin: state.linkedin;		
		
		//only loop over the 'data' prop as this is where new data is added.
        for (const [key, value] of Object.entries(event.data)) {
          //console.log(`logging-keys ${key}: ${value}`);
          state[key] = value;
        }
    }
	
})
.outputState()


//note looping the event object will add all props like this.
		/** 
		 * 
		 for (const [key, value] of Object.entries(event)) {
		 * 
		 * result:
		 {
  "name": "jimi",
  "nrOftimesChanged": 3,
  "linkedin": "https://www.linkedin.com/in/jimi-friis-b729155/",
  "isJson": true,
  "data": {
    "title": "Design thinker and Chief Visionary Officer"
  },
  "body": {
    "title": "Design thinker and Chief Visionary Officer"
  },
  "bodyRaw": "{\r\n  \"title\": \"Design thinker and Chief Visionary Officer\"\r\n}",
  "eventType": "user-updated",
  "streamId": "teststream",
  "sequenceNumber": "3",
  "metadataRaw": "{}",
  "linkMetadataRaw": "",
  "partition": "",
  "metadata_": null
}
		*/
		
```
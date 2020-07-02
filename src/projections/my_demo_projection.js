options({
    resultStreamName: "my_demo_projection_result"
})

fromStream('teststream')
    .when({
        $init: function () {
            return {
                //name: 'not set',
                nrOftimesChanged: 0
            }
        },
        "user-added": function (state, event) {
            //state.name = event.data.name;
            //state.linkedin = event.data.linkedin;

            //only loop over the 'data' prop as this is where new data is added.
            for (const [key, value] of Object.entries(event.data)) {
                //console.log(`logging-keys ${key}: ${value}`);
                state[key] = value;
            }

        },
        "user-updated": function (state, event) {
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
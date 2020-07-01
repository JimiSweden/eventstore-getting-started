   Interface Options



   --int-ip (-IntIp)                           Internal IP Address.



   --ext-ip (-ExtIp)                           External IP Address.



   --http-port                                 The port to run the HTTP server on.



     -HttpPort



   --enable-external-tcp                       Whether to enable external TCP communication



     -EnableExternalTCP



   --int-tcp-port                              Internal TCP Port.



     -IntTcpPort



   --ext-tcp-port                              External TCP Port.



     -ExtTcpPort



   --ext-host-advertise-as                     Advertise External Tcp Address As.



     -ExtHostAdvertiseAs



   --ext-tcp-port-advertise-as                 Advertise External Tcp Port As.



     -ExtTcpPortAdvertiseAs



   --http-port-advertise-as                    Advertise Http Port As.



     -HttpPortAdvertiseAs



   --int-host-advertise-as                     Advertise Internal Tcp Address As.



     -IntHostAdvertiseAs



   --int-tcp-port-advertise-as                 Advertise Internal Tcp Port As.



     -IntTcpPortAdvertiseAs



   --int-tcp-heartbeat-timeout                 Heartbeat timeout for internal TCP sockets



     -IntTcpHeartbeatTimeout



   --ext-tcp-heartbeat-timeout                 Heartbeat timeout for external TCP sockets



     -ExtTcpHeartbeatTimeout



   --int-tcp-heartbeat-interval                Heartbeat interval for internal TCP sockets



     -IntTcpHeartbeatInterval



   --ext-tcp-heartbeat-interval                Heartbeat interval for external TCP sockets



     -ExtTcpHeartbeatInterval



   --gossip-on-single-node                     When enabled tells a single node to run gossip as if it is a cluster


     -GossipOnSingleNode



   --connection-pending-send-bytes-threshold   The maximum number of pending send bytes allowed before a connection is closed.


     -ConnectionPendingSendBytesThreshold



   --connection-queue-size-threshold           The maximum number of pending connection operations allowed before a connection is closed.


     -ConnectionQueueSizeThreshold



   --disable-admin-ui                          Disables the admin ui on the HTTP endpoint



     -DisableAdminUi



   --disable-stats-on-http                     Disables statistics requests on the HTTP endpoint      


     -DisableStatsOnHttp



   --disable-gossip-on-http                    Disables gossip requests on the HTTP endpoint



     -DisableGossipOnHttp



   --enable-trusted-auth                       Enables trusted authentication by an intermediary in the HTTP


     -EnableTrustedAuth



   --disable-internal-tcp-tls                  Whether to disable secure internal tcp communication.  


     -DisableInternalTcpTls



   --disable-external-tcp-tls                  Whether to disable secure external tcp communication.  


     -DisableExternalTcpTls



   --enable-atom-pub-over-http                 Enable AtomPub over HTTP Interface.



     -EnableAtomPubOverHTTP







   Projections Options



   --run-projections                           Enables the running of projections. System runs built-in projections, All runs user projections.


     -RunProjections



                                                 None



                                                 System



                                                 All


   --projection-threads                        The number of threads to use for projections.                                                                                                                

     -ProjectionThreads                                                                                                                                                                                     

   --projections-query-expiry                  The number of minutes a query can be idle before it expires                                                                                                  

     -ProjectionsQueryExpiry                                                                                                                                                                                

   --fault-out-of-order-projections            Fault the projection if the Event number that was expected in the stream differs from what is received. This may happen if events have been deleted or expired

     -FaultOutOfOrderProjections     
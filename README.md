# iCal.PCL
iCal parser in a Portable Class Library

Description
===========
When it comes to the .NET world, the DDay.iCal library is fantastic. I stronly recommend that others check this out.

This is some experiments to understand how hard it is to write such a library, and do it in a portable way
(that is, Windows Phone, etc.). This work was prompted by me spending an hour trying to get DDay.iCal working
in a PCL environment. Of course, I didn't really understand how complex http://tools.ietf.org/html/rfc2445, which
is the complete iCal specification, was.

Initial goals (if this doesn't go off the rails):

  - Deal properly with generic single events
  - No I/O - the person that calls the this library is responsible for feeding it any data
  - Deserialization only
  - Minimal-to-no manipulation once an iCal item has been created.
  - Supports local time and UTC time, but not the VTIMEZONE record

DDay.iCal is rather full featured, especially when it comes to manipulating events, all the various flavors of
iCal events, and reocurring items. This is meant to be simple.

NOTE: Some of my test input .ics files were taken from DDay.iCal. Many thanks to their work for that.
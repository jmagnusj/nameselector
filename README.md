# nameselector

NameSelector is an application for retrieving and randomly selecting names from an XML file, and then displaying the selected names to the user. NameSelector is written in C#, has a simple GUI created in WPF, and selecting names from the XML file is done using XPath.

BACKGROUND
The idea behind the NameSelector application came from experiences in creating exercises in mathematics for students. A frequently recurring situation when creating such exercises is coming up with names for the persons in the exercise texts, i.e. "John and Jane have 12 apples." Rather than using generics (such as "A and B have 12 apples") or arbitrarily choosing names (which would open up for various biases), an approach based on publically available name statistics is possible.

The data in the names XML file is based on publically available statistics from Statistics Sweden (SCB). Website: http://www.scb.se The uploaded XML file contains data on the top 100 female and top 100 male names given to newborns in Sweden in the years 2014-2016. Given this application, adapting the data in the XML file to reflect other countries or years is trivial as long as the statistics is available.

---

**IMPORTANT**
Before running NameSelector, the App.config file has to be edited so that the local file path stored in that configuration file matches the location of names3.xml file.

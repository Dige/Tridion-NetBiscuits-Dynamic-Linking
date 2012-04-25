Dynamic Linking for NetBiscuits
=========

Extended Tridion Content Delivery classes for .Net to enable dynamic linking with NetBiscuits

What does it do?
----------------

Adds an option for Tridion CD classes to output links compatible with NetBiscuits BML. Current version only handles ComponentLinks.

Installation
------------

Front-end (NetBiscuitLinking):

- Add a reference to Tridion.ContentDelivery.dll to NetBiscuitLinking and compile

- Copy the DLL to the bin-folder of your web application

- If you want to use tridion-prefix for the controls make sure that you load this DLL instead of the original one

Back-end (NetBiscuitTemplating):

- Add references to missing Tridion DLLs (you can find them under TRIDION_HOME\bin)

- Upload the DLL to Tridion using TcmUploadAssembly

- Add the TBB to your Page Template after the DWT (i.e. after Output-item is in the package)

Usage
-----

Output standard BML from your template and the included TBB will handle the conversion from the BML to an appropriate ComponentLink-tag which in turn will be converted back into BML with a dynamic link on the front-end.

For example:

	[url="tcm:1-234"]Dynamic Link[/url]

Will be converted by the TBB to
	
	<tridion:ComponentLink ComponentURI="tcm:1-234" NetBiscuitLinkType="URL" LinkText="Dynamic Link" runat="server" />

And when a page containing that tag is served to a user it is converted back to BML with a correct link (assuming there is a page published with that content)

	[url="/something/link.html"]Dynamic Link[/url]

The TBB supports the following types of links

	[url="tcm:1-234"]Dynamic Link[/url]
	[urlnofollow="tcm:1-234"]Dynamic Link[/urlnofollow]
	<link href="tcm:1-234">Dynamic Link</link>
	<cell href="tcm:1-234">Dynamic Link</cell>

Note that cell-tag might contain plenty of styling and additional elements, i.e.

      <cell align="left" width="50%" href="tcm:1-234"><img src="/images/image.jpg" /></cell>

This will be then converted to

      <tridion:ComponentLink ComponentURI="tcm:1-234" NetBiscuitLinkType="Cell" LinkText="" runat="server"><OutputMarkup><cell align="left" width="50%" href="{0}"><img src="/images/image.jpg" /></cell></OutputMarkup></tridion:ComponentLink>

And of course, back to BML on the front-end when serving the page.
    
Related links
-------------

[Getting Started with NetBiscuits] (http://www.asierfernandez.com/2012/03/netbiscuits-helloworld-with-custom-net.html)

Author
-------

Vesa Paakkanen

[Asier Fernandez](http://www.asierfernandez.com/)

License
-------

Licensed under the MIT License.
Dynamic Linking for NetBiscuits
=========

Extended Tridion Content Delivery classes for .Net to enable dynamic linking with NetBiscuits

What does it do?
----------------

Adds an option for Tridion CD classes to output links compatible with NetBiscuits BML. Current version only handles ComponentLinks.

Installation
------------

- Get the project, add a reference to Tridion.ContentDelivery.dll and compile

- Copy the DLL to the bin-folder of your web application

- If you want to use tridion-prefix for the controls make sure that you load this DLL instead of the original one

Usage
-----

Modify your template code to include an additional attribute NetBiscuitLinkType with value URL, URLNoFollow or Link, e.g.

    <tridion:ComponentLink ComponentURI="tcm:1-234" NetBiscuitLinkType="URL" LinkText="Dynamic Link" runat="server" />
    
With attributes URL and URLNoFollow the control outputs BBCode

    [url="link.html"]Dynamic Link[/url]
    [urlnofollow="link.html"]Dynamic Link[/urlnofollow]
    
With attribute Link output is a link-tag

    <link href="link.html">Dynamic Link</link>
    
It is up to you to select the appropriate attribute
    
Related links
-------------

[Getting Started with NetBiscuits] (http://www.asierfernandez.com/2012/03/netbiscuits-helloworld-with-custom-net.html)

Author
-------

Vesa Paakkanen

License
-------

Licensed under the MIT License.
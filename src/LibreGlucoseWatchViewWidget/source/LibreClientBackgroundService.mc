// Copyright(c) 2022 Benito Palacios Sanchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
import Toybox.Application.Storage;
import Toybox.System;
import Toybox.Communications;
import Toybox.Lang;

(:background)
var token as String;

(:background)
var expirationTime;

(:background)
class LibreClientBackgroundService extends System.ServiceDelegate
{
    private var libreLinkUpUrl = "https://api-eu.libreview.io/";
    private var userAgent = "LibreLinkUp";
    private var libreLinkUpVersion = "4.1.1";
    private var libreLinkUpProduct = "llu.ios";

    public var lastMeasurement as Number = 0;

    function initialize() {
        ServiceDelegate.initialize();
    }

    function onTemporalEvent() {
        System.println("LibreClientBackgroundService running");

        if ($.token == null) {
            System.println("Retreiving token from store");
            $.token = Storage.getValue("libretoken");
        }

        // TODO: Check if login is valid (by expiration date)
        if ($.token == null) {
            loginAndQuery();
        } else {
            query();
        }
    }

    function onReceivedConnectionInfo(responseCode as Number, data as Dictionary?) as Void {
        if (responseCode == 200) {
            lastMeasurement = data["data"][0]["glucoseMeasurement"]["ValueInMgPerDl"];
            System.println("Query successful. Last measurement: " + lastMeasurement);
            Background.exit(lastMeasurement);
        } else {
            System.println("Failed to get sugar measurement: " + responseCode);
        }
    }

    function query() as Void {
        System.println("Requesting last measurement");
        var url = libreLinkUpUrl + "llu/connections";
        var options = {
            :method => Communications.HTTP_REQUEST_METHOD_GET,
            :headers => {
                "User-Agent" => userAgent,
                "Content-Type" => Communications.REQUEST_CONTENT_TYPE_JSON,
                "version" => libreLinkUpVersion,
                "product" => libreLinkUpProduct,
                "Accept-Encoding" => "gzip, deflate, br",
                "Connection" => "keep-alive",
                "Pragma" => "no-cache",
                "Cache-Control" => "no-cache",
                "Authorization" => "Bearer " + $.token
            },
            :responseType => Communications.HTTP_RESPONSE_CONTENT_TYPE_JSON
        };

        Communications.makeWebRequest(url, null, options, method(:onReceivedConnectionInfo));
    }

    function onReceivedLogin(responseCode as Number, data as Dictionary?) as Void {
        if (responseCode == 200) {
            System.println("Login successful");

            $.token = data["data"]["authTicket"]["token"];
            Storage.setValue("libretoken", $.token);
            System.println("Token: " + $.token);

            query();
        } else {
            System.println("Failed to login: " + responseCode);
        }
    }

    function loginAndQuery() as Void {
        System.println("Requesting login");
        var url = libreLinkUpUrl + "llu/auth/login";
        var options = {
            :method => Communications.HTTP_REQUEST_METHOD_POST,
            :headers => {
                "User-Agent" => userAgent,
                "Content-Type" => Communications.REQUEST_CONTENT_TYPE_JSON,
                "version" => libreLinkUpVersion,
                "product" => libreLinkUpProduct,
                "Accept-Encoding" => "gzip, deflate, br",
                "Connection" => "keep-alive",
                "Pragma" => "no-cache",
                "Cache-Control" => "no-cache"
            },
            :responseType => Communications.HTTP_RESPONSE_CONTENT_TYPE_JSON
        };

        var params = {                                              // set the parameters
            "email" => "YOUR_EMAIL",
            "password" => "YOUR_PASSWORD"
        };

        Communications.makeWebRequest(url, params, options, method(:onReceivedLogin));
    }
}

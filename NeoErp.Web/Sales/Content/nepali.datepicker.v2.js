ndpAttr = {};
calendarVisible = '';
ignoreMouseUp = false;
ndpData = [];
(function( $ ) {
  $.fn.nepaliDatePicker = function(attributes) {
  	attributes = typeof attributes !== 'undefined' ? attributes : {};
		ndpAttr = attributes;
		this.each(function() {
			var id = $(this).attr('id'); 
			$(this).addClass('ndp-nepali-calendar');
			ndpData[id] = attributes;
			if(ndpAttr.onFocus !== false)
				$(this).attr('onfocus', "showNdpCalendarBox('" + id + "')");
			if(ndpAttr.ndpTriggerButton)
				$(this).after('<button type="button" class="ndp-click-trigger '+(ndpAttr.ndpTriggerButtonClass?ndpAttr.ndpTriggerButtonClass:'')+'" onclick="showNdpCalendarBox(&quot;'+id+'&quot;)">'+(ndpAttr.ndpTriggerButtonText?ndpAttr.ndpTriggerButtonText:'Pick Date')+'</button>');
	  });
		$("body").append(calendarDivString);

		$('.ndp-nepali-calendar, #ndp-nepali-box').hover(function(){ 
			mouse_is_inside = true; 
		}, function(){ 
			mouse_is_inside = false; 
		});

		$("html").mouseup(function(){ 
			if(!$(event.target).is('.ndp-click-trigger')){
				if(calendarVisible && !mouse_is_inside) {
					hideCalendarBox(false);
				}
			}
		});
  };
})( jQuery );
	
	var mouse_is_inside = false;
	
	var calendarDivString = '<div id="ndp-nepali-box" class="ndp-corner-all" style="display:none">';
	calendarDivString += '<span id="ndp-target-id" style="display:none"></span>';
	calendarDivString += '<div class="ndp-corner-all ndp-header">';
	calendarDivString += '<a href="javascript:void(0)" id="prev" title="Previous Month" class="ndp-prev"></a>';
	calendarDivString += '<a href="javascript:void(0)" id="next" title="Next Month" class="ndp-next"></a>';
	calendarDivString += '<span id="currentMonth"></span>';
	calendarDivString += '</div>';
	calendarDivString += '<div id="npd-table-div">';
	calendarDivString += '<table>';
	calendarDivString += '<tr class="ndp-days">';
	calendarDivString += '<th>आ</th>';
	calendarDivString += '<th>सो</th>';
	calendarDivString += '<th>मं</th>';
	calendarDivString += '<th>बु</th>';
	calendarDivString += '<th>बि</th>';
	calendarDivString += '<th>शु</th>';
	calendarDivString += '<th>श</th>';
	calendarDivString += '</tr>';
	calendarDivString += '</table>';
	calendarDivString += '</div>';
	calendarDivString += '</div>';
	
	function showNdpCalendarBox(id){
		if(calendarVisible){
			hideCalendarBox(false);
		}else{
			if(ndpData[id]){
				ndpAttr = ndpData[id];
			}

			var val = $('#'+id).val();
			$('#ndp-target-id').html(id);
			var idPosition = $('#'+id).offset();
			$('#ndp-nepali-box').css('top', idPosition.top + $('#'+id).outerHeight());
			$('#ndp-nepali-box').css('left', idPosition.left);
			showCalendar(val);
			calendarVisible = true;
		}
	}
	
	function setSelectedDay(value){
		var ndp_target_id = $('#ndp-target-id').html();
		$('#'+ndp_target_id).val(value);

		if(ndpAttr.ndpEnglishInput){
			$('#'+ndpAttr.ndpEnglishInput).val(BS2AD(value));
		}

		hideCalendarBox();
	}
	
	function showCalendar(val){
		$("#ndp-nepali-box table").find("tr:gt(0)").remove();
		if(val === ""){
			$('#ndp-nepali-box table').append(getDateTable(''));
		}else{
			$('#ndp-nepali-box table').append(getDateTable(val));
		}
		if($('#ndp-nepali-box').css('display')=='block') $('#ndp-nepali-box').hide();
		$('#ndp-nepali-box').fadeIn(100);
		var mouse_is_inside = false;
	}
	
	function getDateTable(date){
		var monthParameters = "", returnString = "";
		if(date === ""){
			var currentNepaliDate = getNepaliDate();
			monthParameters = getMonthParameters(currentNepaliDate);
			
			returnString = getDateRows(monthParameters[0], monthParameters[1], monthParameters[2], monthParameters[3], monthParameters[4]);
			return returnString;
		}else{
			monthParameters = getMonthParameters(date);
			
			returnString = getDateRows(monthParameters[0], monthParameters[1], monthParameters[2], monthParameters[3], monthParameters[4]);
			return returnString;
		}
	}
	
	// Get parameters for Date Rows
	function getMonthParameters(date){
		// Create Nepali Date Parts
		var currentNepDateParts = date.split('-');
		var nepaliYear = currentNepDateParts[0];
		var nepaliMonth = currentNepDateParts[1]; 
			// Set Values
			$('#currentMonth').html(getNepaliMonth(nepaliMonth-1) + '&nbsp;' + englishNo2Nep(nepaliYear));
			nYear = pYear = nepaliYear; 
			nMonth = (parseInt(nepaliMonth,10)+1); if(nMonth > 12){ nYear++; nMonth = '01';}
			pMonth = (parseInt(nepaliMonth,10)-1); if(pMonth < 1){ pYear--; pMonth = '12';}
			$('#prev').attr('onclick', "showCalendar('"+pYear+"-"+pMonth+"-"+"01')");
			$('#next').attr('onclick', "showCalendar('"+nYear+"-"+nMonth+"-"+"01')");
		var nepaliDay = currentNepDateParts[2];
		// Get total days in current nepali month
		var totalNepaliMonthDays = numberOfBsDays(nepaliYear, nepaliMonth-1);
		
		var d = new NepaliDateConverter();
		// Get current month start date
		var currentNepaliMonthStartDate = nepaliMonth + '/' + '1' + '/' +  nepaliYear;
		var currentEnglishMonthStartDate = d.bs2ad(currentNepaliMonthStartDate);
		var currentEnglishMonthStartDateParts = currentEnglishMonthStartDate.split('-');
		var currentEnglishMonthStartYear = currentEnglishMonthStartDateParts[0];
		var currentEnglishMonthStartMonth = currentEnglishMonthStartDateParts[1];
		var currentEnglishMonthStartDay = currentEnglishMonthStartDateParts[2];
		
		var currentEnglishStartDate = new Date(currentEnglishMonthStartYear, currentEnglishMonthStartMonth-1, currentEnglishMonthStartDay);
		var currentEnglishStartDay = currentEnglishStartDate.getDay();
		/* Week Starts at 0: 0=Sunday */
		
		return new Array(currentEnglishStartDay, totalNepaliMonthDays, nepaliYear, nepaliMonth, nepaliDay);
	}
	
	// Returns the actual Table rows
	function getDateRows(startDayCode, totalDays, currentYear, currentMonth, currentDay){
		// For Current Day Highlight
		var nepaliDateNow = getNepaliDate();
		var currentNepDateParts = nepaliDateNow.split('-');
		var nepaliYear = currentNepDateParts[0];
		var nepaliMonth = get2DigitNo(currentNepDateParts[1]);
		var nepaliDay = get2DigitNo(currentNepDateParts[2]);		
		
		var returnString = "";
		for(var i = 0; i < startDayCode + totalDays; i++){
			if((i % 7) === 0 ) returnString += "<tr>";
			var nepDateVal = (i - startDayCode + 1);
			var loopDateValue = currentYear.toString() + '-' + get2DigitNo(currentMonth) + '-' + get2DigitNo(nepDateVal);
			
			var className = "";
			if(get2DigitNo(currentMonth) == nepaliMonth && nepaliDay == get2DigitNo(nepDateVal)){
				className = "ndp-selected";
			}else if(nepDateVal == currentDay){
				className = "ndp-current";
			}else{
				className = "ndp-date";
			}
			
			if(i < startDayCode) returnString += "<td></td>\n";
			else {
				returnString += "<td class='"+className+"'>";
				returnString += "<a href='javascript:void(0)' onclick=\"setSelectedDay('"+loopDateValue+"')\">" + englishNo2Nep(nepDateVal) + "</a>";
				returnString += "</td>\n";
			}
			if((i % 7) == 6 ) returnString += "</tr>\n";
		}
		return returnString;
	}
	
	function hideCalendarBox(inside){
		inside = typeof inside !== 'undefined' ? inside : true;
		$('#ndp-nepali-box').fadeOut(100);
		calendarVisible = false;
		if(inside && ndpAttr.onChange){
			ndpAttr.onChange();
		}
	}

// Get 2 digit Number
	function get2DigitNo(no){
		no = parseInt(no, 10);
		if(no < 10){
			return '0' + no.toString();
		}else{
			return no.toString();
		}
	}
	
	//Get English Months
	function getMonths(){
		var months = new Array('January','February','March','April','May', 'June','July','August','September','October','November','December');
		return months;
	}
	
	//Get English Months
	function getNepaliMonths(){
		var months = new Array('Baisakh','Jestha','Ashar','Shrawan','Bhadra', 'Ashoj','Kartik','Mangsir','Poush','Magh','Falgun','Chaitra');
		return months;
	}
	
	//Get Nepali Day Short
	function getNepaliDaysShort(){
		var months = new Array('आ', 'सो', 'मं', 'बु', 'बि', 'शु', 'श');
		return months;
	}
	
	//Get Nepali Months
	function getNepaliMonth(id){
		id = parseInt(id, 10);
		var months = new Array('बैशाख','जेठ','अषाढ','श्रावण','भाद्र', 'आश्विन','कार्तिक','मङ्सिर','पौष','माघ','फाल्गुन','चैत्र');
		return months[id];
	}
	
	//Get Current Day
	function getCurrentDayName(){
		var date = new Date();
		var day = date.getDay();
		var day_name=new Array(7);
		day_name[0]="Sunday";
		day_name[1]="Monday";
		day_name[2]="Tuesday";
		day_name[3]="Wednesday";
		day_name[4]="Thursday";
		day_name[5]="Friday";
		day_name[6]="Saturday";
		return day_name[day];
	}
	
	// Get Day from date
	function getDayFromDate(date){
		var x = date.split('-');
		var day = x[2];
		var month = x[1];
		var year = x[0];
		var nDate = new Date(year, month-1, day);
		var dayCode = nDate.getDay();
		var weekDays = new Array("Sun","Mon","Tue","Wed","Thu","Fri","Sat");
		return weekDays[dayCode];
	}
	
	//Get Max no of days in a Nepali month
	function numberOfBsDays(year,month){
	    var d = new NepaliDateConverter();
		return d.bs[year][month];
	}
	
	//Get Max no of days in an English month
	function numberOfDays(year, month) {
		var d = new Date(year, month, 0);
		return d.getDate();
	}

	function AD2BS(adDate){
		var d = new NepaliDateConverter();
		return d.ad2bs(getNepaliFormat(adDate));
	}

	function BS2AD(bsDate){
		var d = new NepaliDateConverter();
		return d.bs2ad(getNepaliFormat(bsDate));
	}
	
	//Get Nepali Date 
	function getNepaliDate(){
		var d = new NepaliDateConverter();
		return d.ad2bs(getDateInNo('/'));
	}
	
	//Get Current Date in 26 November, 2011
	function getDateInWords(){
		var months = getMonths();
		var date = new Date();
		var day = date.getDate();
		var month = date.getMonth() + 1;
		var yy = date.getYear();
		var year = (yy < 1000) ? yy + 1900 : yy;
		return day + " " + months[month] + ", " + year;
	}
	
	//Get Current date in 11/30/2011
	function getDateInNo(separator){
		var date = new Date();
		var day = date.getDate();
		var month = date.getMonth() + 1;
		var year = date.getFullYear();
		return month + separator + day + separator + year;
	}

	//convert YYYY-MM-DD to MM/DD/YYYY
	function getNepaliFormat(_date){
		var x = _date.split('-');
		var day = x[2];
		var month = x[1];
		var year = x[0];
		return month+'/'+day+'/'+year;
	}
	
	// Get AD Date in Words
	function getAdDateInWords(AdDate){
		var months = getMonths();
		var x = AdDate.split('-');
		var day = x[2];
		var month = x[1];
		var year = x[0];
		return day + " " + months[month] + ", " + year;
	}
	
	// Get Nepali Date in Words
	function getNepaliDateInWords(NepDate){
		var months = getNepaliMonths();
		var x = NepDate.split('-');
		var day = x[2];
		var month = x[1];
		var year = x[0];
		return day + " " + months[month] + ", " + year;
	}
	
	// Get current Year
	function getCurrentYear(){
		var date = new Date();
		var year = date.getFullYear();
		return year;
	}
	
	// Get current Month
	function getCurrentMonth(){
		var date = new Date();
		var month = date.getMonth() + 1;
		return month;
	}
	
	// Get current day
	function getCurrentDay(){
		var date = new Date();
		var day = date.getDate();
		return day;
	}
	
	// Make Array
	function makeArray() {
		for (i = 0; i<makeArray.arguments.length; i++)
		this[i + 1] = makeArray.arguments[i];
	}
	
	// Convert English No Character to Nepali
	function englishNo2Nep(no){
		no = no.toString();
		var nepNo = '';
		for(var i=0;i<no.length;i++){
			nepNo+=convertNos(no[i]);
		}
		return nepNo;
	}
	
	// Returns curresponding Nepali/English No
	function convertNos(n_no){
		switch(n_no){
			case '०':
				return 0;
			case '१':
				return 1;
			case '२':
				return 2;
			case '३':
				return 3;
			case '४':
				return 4;
			case '५':
				return 5;
			case '६':
				return 6;
			case '७':
				return 7;
			case '८':
				return 8;
			case '९':
				return 9;
			case '0':
				return '०';
			case '1':
				return '१';
			case '2':
				return '२';
			case '3':
				return '३';
			case '4':
				return '४';
			case '5':
				return '५';
			case '6':
				return '६';
			case '7':
				return '७';
			case '8':
				return '८';
			case '9':
				return '९';
		}
	}
	
	//Find Sum of array
	arraySum = function(o){
		for(var s = 0, i = o.length; i; s += o[--i]);
		return s;
	};
	
	////// BEGIN LICENSE
	// Copyright (C) 2011 Shritesh Bhattarai shriteshb@gmail.com
	// This program is free software: you can redistribute it and/or modify it 
	// under the terms of the GNU General Public License version 3, as published 
	// by the Free Software Foundation.
	// 
	// This program is distributed in the hope that it will be useful, but 
	// WITHOUT ANY WARRANTY; without even the implied warranties of 
	// MERCHANTABILITY, SATISFACTORY QUALITY, or FITNESS FOR A PARTICULAR 
	// PURPOSE.  See the GNU General Public License for more details.
	// 
	// You should have received a copy of the GNU General Public License along 
	// with this program.  If not, see <http://www.gnu.org/licenses/>.
	////// END LICENSE
	
	function NepaliDateConverter(){
		this.bs_date_eq = "09/17/2000";
		this.ad_date_eq = "01/01/1944";
		this.bs = [];
		
		this.bs[2000]=new Array(30,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2001]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2002]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2003]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2004]=new Array(30,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2005]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2006]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2007]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2008]=new Array(31,31,31,32,31,31,29,30,30,29,29,31);
		this.bs[2009]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2010]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2011]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2012]=new Array(31,31,31,32,31,31,29,30,30,29,30,30);
		this.bs[2013]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2014]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2015]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2016]=new Array(31,31,31,32,31,31,29,30,30,29,30,30);
		this.bs[2017]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2018]=new Array(31,32,31,32,31,30,30,29,30,29,30,30);
		this.bs[2019]=new Array(31,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2020]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2021]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2022]=new Array(31,32,31,32,31,30,30,30,29,29,30,30);
		this.bs[2023]=new Array(31,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2024]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2025]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2026]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2027]=new Array(30,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2028]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2029]=new Array(31,31,32,31,32,30,30,29,30,29,30,30);
		this.bs[2030]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2031]=new Array(30,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2032]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2033]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2034]=new Array(31,32,31,32,31,30,30,30,29,29,30,31); 
		this.bs[2035]=new Array(30,32,31,32,31,31,29,30,30,29,29,31);
		this.bs[2036]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2037]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2038]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2039]=new Array(31,31,31,32,31,31,29,30,30,29,30,30);
		this.bs[2040]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2041]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2042]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2043]=new Array(31,31,31,32,31,31,29,30,30,29,30,30);
		this.bs[2044]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2045]=new Array(31,32,31,32,31,30,30,29,30,29,30,30);
		this.bs[2046]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2047]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2048]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2049]=new Array(31,32,31,32,31,30,30,30,29,29,30,30);
		this.bs[2050]=new Array(31,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2051]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2052]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2053]=new Array(31,32,31,32,31,30,30,30,29,29,30,30);
		this.bs[2054]=new Array(31,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2055]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2056]=new Array(31,31,32,31,32,30,30,29,30,29,30,30);
		this.bs[2057]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2058]=new Array(30,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2059]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2060]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2061]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2062]=new Array(30,32,31,32,31,31,29,30,29,30,29,31);
		this.bs[2063]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2064]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2065]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2066]=new Array(31,31,31,32,31,31,29,30,30,29,29,31);
		this.bs[2067]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2068]=new Array(31,31,32,32,31,30,30,29,30,29,30,30);
		this.bs[2069]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2070]=new Array(31,31,31,32,31,31,29,30,30,29,30,30);
		this.bs[2071]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2072]=new Array(31,32,31,32,31,30,30,29,30,29,30,30);
		this.bs[2073]=new Array(31,32,31,32,31,30,30,30,29,29,30,31);
		this.bs[2074]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2075]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2076]=new Array(31,32,31,32,31,30,30,30,29,29,30,30);
		this.bs[2077]=new Array(31,32,31,32,31,30,30,30,29,30,29,31);
		this.bs[2078]=new Array(31,31,31,32,31,31,30,29,30,29,30,30);
		this.bs[2079]=new Array(31,31,32,31,31,31,30,29,30,29,30,30);
		this.bs[2080]=new Array(31,32,31,32,31,30,30,30,29,29,30,30);
		this.bs[2081]=new Array(31,31,32,32,31,30,30,30,29,30,30,30);
		this.bs[2082]=new Array(30,32,31,32,31,30,30,30,29,30,30,30);
		this.bs[2083]=new Array(31,31,32,31,31,30,30,30,29,30,30,30);
		this.bs[2084]=new Array(31,31,32,31,31,30,30,30,29,30,30,30);
		this.bs[2085]=new Array(31,32,31,32,30,31,30,30,29,30,30,30);
		this.bs[2086]=new Array(30,32,31,32,31,30,30,30,29,30,30,30);
		this.bs[2087]=new Array(31,31,32,31,31,31,30,30,29,30,30,30);
		this.bs[2088]=new Array(30,31,32,32,30,31,30,30,29,30,30,30);
		this.bs[2089]=new Array(30,32,31,32,31,30,30,30,29,30,30,30);
		this.bs[2090]=new Array(30,32,31,32,31,30,30,30,29,30,30,30);
		
		this.count_ad_days = count_ad_days;
		this.count_bs_days = count_bs_days;
		this.add_bs_days = add_bs_days;
		this.add_ad_days = add_ad_days;
		this.bs2ad = bs2ad;
		this.ad2bs = ad2bs;
	}
			
	//Format: MM/DD/YYYY
	function count_ad_days(start_date, end_date) {
		var one_day=1000*60*60*24;
		var x=start_date.split("/");     
		var y=end_date.split("/");
		
		x[2] = Number(x[2]);
		x[1] = Number(x[1]);
		x[0] = Number(x[0]);
		
		y[2] = Number(y[2]);
		y[1] = Number(y[1]);
		y[0] = Number(y[0]);
		
		var date1=new Date(x[2],(x[0]-1),x[1]);
		var date2=new Date(y[2],(y[0]-1),y[1]);
		
		var Diff=Math.ceil((date2.getTime()-date1.getTime())/(one_day));
		return Diff;
	}
			
	//Format: MM/DD/YYYY
	function count_bs_days(start_date, end_date){
		var x=start_date.split("/");     
		var y=end_date.split("/");
		
		var start_year = Number(x[2]);
		var start_month = Number(x[0]);
		var start_days = Number(x[1]);
		
		var end_year = Number(y[2]);
		var end_month = Number(y[0]);
		var end_days = Number(y[1]);
		
		/*
		Its not the piece of algorithm, but it works for this program..
		
		1) First add total days in all the years
			
		2) Subtract the days from first (n-1) months of the beginning year 
		
		3) Add the number of days from the last month of the beginning year
			 
		4) Subtract the days from the last months from the end year
		
		5) Add the beginning days excluding the day itself
		
		6) Add the last remaining days excluding the day itself 
		*/
		
		var days = 0;
		var i = 0;
		
	//	1) First add total days in all the years
		for(i = start_year; i <= end_year; i++)
		{ days += arraySum(this.bs[i]); }
		
	//  2) Subtract the days from first (n-1) months of the beginning year
		for(i = 0; i < start_month; i++)
		{ days -= this.bs[start_year][i]; }
		
	//	3) Add the number of days from the last month of the beginning year
		days += this.bs[start_year][12-1];
	
	//	4) Subtract the days from the last months from the end year
		for(i = end_month - 1; i < 12; i++)
		{ days -= this.bs[end_year][i]; }
	
	//	5) Subtract the beginning days excluding the day itself
		days -= start_days + 1;
	
	//	6) Add the last remaining days excluding the day itself 
		days += end_days - 1;
		
		return days;
	}
			
	//  MM/DD/YYYY returns YYYY-MM-DD
	function add_ad_days(ad_date, num_days){
		var d = new Date(ad_date);
		d.setDate(d.getDate() + num_days);
		
		ad_month = d.getMonth()+1;
		ad_day = d.getDate();
		return d.getFullYear() + '-' + (ad_month<10?'0'+ad_month.toString():ad_month) + "-"+(ad_day<10?'0'+ad_day.toString():ad_day);
	}
	
	//  DD/MM/YYYY returns YYYY-MM-DD
	function add_bs_days(bs_date, num_days){
		var x=bs_date.split("/");     
		
		var bs_year = Number(x[2]);
		var bs_month = Number(x[0]);
		var bs_days = Number(x[1]);
		
		/*
		Algorithm: 
		1) Add the total number of days to the original days
		
		2) Until the number of days becomes applicable to the current month, subtract the days by the number of days in the current month and increase the month
			
		3) If month reaches 12, increase the year by 1 and set the month to 1
		*/
	//	1) Add the total number of days to the original days
		bs_days += num_days;
		
	//	2) Until the number of days becomes applicable to the current month, subtract the days by the number of days in the current month and increase the month
		while(bs_days > this.bs[bs_year][bs_month - 1])
		{
			bs_days -= this.bs[bs_year][bs_month - 1];
			bs_month++;
			//3) If month reaches 12, increase the year by 1 and set the month to 1
				if(bs_month > 12)
				{
					bs_month = 1;
					bs_year++;
				}
		}
		return bs_year + '-' + (bs_month<10?'0'+bs_month.toString():bs_month) + '-' + (bs_days<10?'0'+bs_days.toString():bs_days);
	}
	
	function bs2ad(bs_date){
		days_count = this.count_bs_days(this.bs_date_eq, bs_date);
		return this.add_ad_days(this.ad_date_eq, days_count);
	}
	
	function ad2bs(ad_date){
		days_count = this.count_ad_days(this.ad_date_eq, ad_date);
		return this.add_bs_days(this.bs_date_eq, days_count);
	}
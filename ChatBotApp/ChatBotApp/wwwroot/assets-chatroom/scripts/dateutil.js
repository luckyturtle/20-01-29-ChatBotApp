function getDateObj(ds) {
    var a = ds.split("T");
    var b = a[0].split("-");
    var c = a[1].split(".")[0].split(":");
    return new Date(b[0], b[1] - 1, b[2], c[0], c[1], c[2]);
}
function pad(s) { return (s < 10) ? '0' + s : s; }
function getValitDateFormat(date) {
    var day = date.getDate();       // yields date
    var month = date.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
    var year = date.getFullYear();  // yields year
    var hour = date.getHours();     // yields hours
    var minute = date.getMinutes(); // yields minutes
    var second = date.getSeconds(); // yields seconds
    // After this construct a string with the above results as below
    var time = pad(day) + "/" + pad(month) + "/" + year + " " + pad(hour) + ':' + pad(minute) + ':' + pad(second);
    return time;
}
function getLastTime(dtt) {//  2/1/2020/0:11:11:22
    //11:01 AM    |    June 9
    var dateObj = new Date();
    var month = dateObj.getUTCMonth() + 1; //months from 1-12
    var day = dateObj.getUTCDate();
    var year = dateObj.getUTCFullYear();
    var week = dateObj.getDay();//sun-0
    var time = dateObj.getHours() + ":" + dateObj.getMinutes() + ":" + dateObj.getSeconds();

    var weeks = (new String("Sun,Mon,Thu,Wen,Thu,Fri,Sat")).split(",");
    var res = month + "/" + day + "/" + year;
    var str = dtt.split("/");
    if (str[0] == month && str[2] == year) {
        if (str[1] == day) {
            var str1 = str[4].split(":");
            res = str[4] + ":" + str[5] + " " + getAmOrPm(str1[0]);
        }
        else res = weeks[str[3]];
    }
    return res;
}
function getNowTime(d) {//  2/1/2020/0:11:11:22
    var dateObj = d;
    var month = dateObj.getUTCMonth() + 1; //months from 1-12
    var day = dateObj.getUTCDate();
    var year = dateObj.getUTCFullYear();
    var week = dateObj.getDay();//sun-0
    var time = dateObj.getHours() + ":" + dateObj.getMinutes();
    var nowdate = dateObj.getHours() + ":" + dateObj.getMinutes() + " " + getAmOrPm(dateObj.getHours()) + " | " + month + "/" + day + "/" + year;
    return nowdate;
}
function getAmOrPm(hours) {
    var hours = (hours + 24 - 2) % 24;
    var mid = 'AM';
    if (hours == 0) { //At 00 hours we need to show 12 am
        hours = 12;
    }
    else if (hours > 12) {
        hours = hours % 12;
        mid = 'PM';
    }
    return mid
}
/* Get ISO week in month, based on first Monday in month
** @param {Date} date - date to get week in month of
** @returns {Object} month: month that week is in week: week in month
*/
function getISOWeekInMonth(date) {
    // Copy date so don't affect original
    var d = new Date(+date);
    if (isNaN(d)) return 0;
    // Move to previous Monday
    d.setDate(d.getDate() - d.getDay() + 1);
    // Week number is ceil date/7
    return Math.ceil(d.getDate() / 7);
}
/**
 * getLastTimeFromServerTime from  2/1/2020 02:11:22 AM
 */
function getLastTimeFromServerTime(d) {//  2/1/2020 02:11:22 AM
    var weeks = (new String("Sun,Mon,Thu,Wen,Thu,Fri,Sat")).split(",");
    var arr = d.split(" ");
    var date = arr[0];
    var time_arr = (arr.length > 1 ? arr[1] : "00:00:00").split(":");
    var time = time_arr[0] + ":" + time_arr[1];
    var apm = arr.length > 2 ? arr[2] : "AM";
    var date_arr = date.split("/");
    var date_obj = new Date(date_arr[2], date_arr[0] - 1, date_arr[1], time[0], time_arr[1]);//.toLocaleString("en-US", { timeZone: "America/New_York" });
    var today_obj = new Date();//.toLocaleString("en-US", { timeZone: "America/New_York" });;
    if (date_arr[0] == (today_obj.getUTCMonth() + 1) && date_arr[2] == today_obj.getUTCFullYear()) {
        if (date_arr[1] == today_obj.getUTCDate()) {
            return time + " " + apm;
        } else if (getISOWeekInMonth(date_obj) == getISOWeekInMonth(today_obj)) {
            return weeks[date_obj.getDay()];
        } else {
            return date_arr[0] + "/" + date_arr[1];
        }
    } else return date;
}
<%@ Control Language="C#" %>

<h3 style="margin-bottom: -5px; margin-top: 20px;">Most Popular Items</h3>
<div id="piechart">
    <strong>Unable to display the chart</strong>
</div>

<script type="text/javascript">
    // <![CDATA[
        var so = new SWFObject("reporting/ampie.swf", "ampie", "100%", "250", "4", "#ffffff");
        so.addVariable("path", "reporting/");
        so.addVariable("settings_file", escape("reporting/piegraph.xml"));
        so.addVariable("data_file", escape("reporting/charts.ashx?report=MostPopularPosts"));
        so.addVariable("preloader_color", "#999999");
		so.addParam('wmode', 'transparent');
        so.write("piechart");
    // ]]>
</script>
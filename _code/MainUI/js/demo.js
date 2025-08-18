/**
 * Demo
 * Modernized for jQuery 3.7.1
 */

$(function () {
    // Treeview: Example 1
    $("#navigation").treeview({
        persist: "location",
        collapsed: true,
        unique: true
    });

    // Treeview: Example 2 – Dynamic branch addition
    $("#browser").treeview();

    $("#add").on("click", function () {
        const newBranchHtml = `
            <li>
                <span class="folder">New Sublist</span>
                <ul>
                    <li><span class="file">Item1</span></li>
                    <li><span class="file">Item2</span></li>
                </ul>
            </li>`;

        const newBranchTop = $(newBranchHtml).appendTo("#browser");
        $("#browser").treeview({ add: newBranchTop });

        const newBranchInner = $(newBranchHtml).addClass("closed").prependTo("#folder21");
        $("#browser").treeview({ add: newBranchInner });
    });

    // Treeview: Example 3
    $("#red").treeview({
        animated: "fast",
        collapsed: true,
        unique: true,
        persist: "cookie",
        toggle: function () {
            if (window.console) {
                console.log("%o was toggled", this);
            }
        }
    });

    // Treeview: Example 4 – Shared control
    $("#black, #gray").treeview({
        control: "#treecontrol",
        persist: "cookie",
        cookieId: "treeview-black"
    });
});

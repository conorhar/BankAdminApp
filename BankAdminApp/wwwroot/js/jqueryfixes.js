$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}

$.validator.methods.number = function(value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
}




$.validator.addMethod("validaccountno",
    function (value, element, params) {

        if (!isNaN(value) && value.length === 8) {
            return true;
        }

        return false;
    });

$.validator.unobtrusive.adapters.addBool("validaccountno");



$.validator.addMethod("validbankcode",
    function (value, element, params) {

        if (value !== "" && !/[^a-zA-Z]/.test(value) && value.toUpperCase() === value && value.length === 2) {
            return true;
        }

        return false;
    });

$.validator.unobtrusive.adapters.addBool("validbankcode");
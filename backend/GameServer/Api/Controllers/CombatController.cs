/*
    [HttpGet("{id}/get-resistance")]
    public ActionResult<ResistanceDto> GetResistance(string id, [FromBody] string damageType)
    {
        var result = _service.GetResistanceMultiplier(id, damageType);
        if (result is null)
        {
            return NotFound(IdNotFound(id));
        }
        if (result.Resistance.Length == 0 && result.Value == 0)
        {
            return ValidationProblem($"{damageType} is not a valid Damage Type");
        }
        return Ok(result);
    }
*/